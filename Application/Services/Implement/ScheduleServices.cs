using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using AutoMapper;
using Azure.Core;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Implement;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using Microsoft.Extensions.Configuration;
using OneOf.Types;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AccountResponse;
using static Infrastructure.ViewModel.Response.CropRequirementResponse;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;
using static Infrastructure.ViewModel.Response.ScheduleResponse;
using ResponseDTO = Infrastructure.ViewModel.Response.ScheduleResponse.ResponseDTO;

namespace Application.Services.Implement
{
    public class ScheduleServices : IScheduleServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _account;
        private readonly ICropRepository _cropRepository;
        private readonly IFarmActivityRepository _farmActivityRepository;
        private readonly IFarmRepository _farmRepository;
        private readonly JWTUtils _jwtUtils;
        private readonly IInventoryService _inventory;
        public ScheduleServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper
                , IAccountRepository account, ICropRepository cropRepositorym, IFarmActivityRepository farmActivityRepository
                , IFarmRepository farmRepository, JWTUtils jwtUtils, IInventoryService inventory)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _account = account;
            _cropRepository = cropRepositorym;
            _farmActivityRepository = farmActivityRepository;
            _farmRepository = farmRepository;
            _jwtUtils = jwtUtils;
            _inventory = inventory;
        }
        /// <summary>
        /// Validate nghiệp vụ khi tạo/cập nhật Schedule
        /// Trả về (true, "") nếu hợp lệ
        /// Trả về (false, message) nếu có lỗi
        /// </summary>
        private async Task<(bool IsValid, string ErrorMessage)> ValidateScheduleRequestAsync(ScheduleRequest request)
        {
            // 1. Validate input cơ bản từ DTO
            if (!request.StartDate.HasValue)
                return (false, "Ngày bắt đầu lịch trình là bắt buộc.");

            if (request.Quantity <= 0)
                return (false, "Số lượng trồng phải lớn hơn 0.");

            if (request.Status == null)
                return (false, "Trạng thái lịch trình là bắt buộc.");

            var today = DateOnly.FromDateTime(_currentTime.GetCurrentTime().ToDateTime(TimeOnly.MinValue));

            if (request.StartDate.Value < today)
            {
                return (false, "Ngày bắt đầu không được trong quá khứ. Chỉ được chọn từ hôm nay trở đi.");
            }

            // 2. Kiểm tra Crop và CropRequirement
            var crop = await _unitOfWork.cropRepository.GetByIdAsync(request.CropId);
            if (crop == null)
                return (false, "Không tìm thấy cây trồng.");

            if (crop.CropRequirement == null || !crop.CropRequirement.Any())
                return (false, "Cây trồng này chưa có thông tin yêu cầu chăm sóc (CropRequirement).");

            int totalDays = crop.CropRequirement
                .Where(r => r.EstimatedDate.HasValue)
                .Sum(r => r.EstimatedDate.Value);

            if (totalDays <= 0)
                return (false, "Tổng thời gian chăm sóc của cây trồng không hợp lệ.");

            // 3. Tính EndDate và kiểm tra chồng lịch ACTIVE
            DateOnly estimatedEndDate = request.StartDate.Value.AddDays(totalDays);

            bool hasOverlapping = await _unitOfWork.scheduleRepository.HasOverlappingActiveScheduleAsync(
                //farmId: request.FarmId,
                startDate: request.StartDate.Value,
                endDate: estimatedEndDate);

            if (hasOverlapping)
                return (false,
                    $"Nông trại này đã có lịch trình đang hoạt động chồng chéo từ {request.StartDate.Value:dd/MM/yyyy} đến {estimatedEndDate:dd/MM/yyyy}.");

            // Nếu mọi thứ đều hợp lệ
            return (true, string.Empty);
        }

        #region Schedule mới
        public async Task<ResponseDTO> CreateSchedulesAsync(ScheduleRequest request)
        {
            try
            {
                // Gọi hàm validate
                var (isValid, errorMessage) = await ValidateScheduleRequestAsync(request);

                if (!isValid)
                    return new ResponseDTO(Const.ERROR_EXCEPTION, errorMessage);

                // Kiểm tra quyền
                var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
                if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }

                var getCrop = await _unitOfWork.cropRepository.GetByIdAsync(request.CropId);
                if(getCrop == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Không tìm thấy cây trồng.");
                }
                int totalTimes = getCrop.CropRequirement?
                                    .Where(r => r.EstimatedDate.HasValue)
                                    .Sum(r => r.EstimatedDate.Value) ?? 0;

                // Tính ngày kết thúc dự kiến nếu có StartDate từ Schedule
                DateOnly? estimatedEndDate = null;
                if (request?.StartDate.HasValue == true) 
                {
                    estimatedEndDate = request.StartDate.Value.AddDays(totalTimes);
                }
                // Tạo schedule mới
                var schedule = _mapper.Map<Schedule>(request);
                schedule.ManagerId = getCurrentUser.AccountId;
                schedule.CreatedAt = _currentTime.GetCurrentTime();
                schedule.currentPlantStage = PlantStage.Preparation;
                schedule.EndDate = estimatedEndDate;
                // Kiểm tra yêu cầu cây trồng
                var getCropRequirement = await _cropRepository.GetByIdAsync(request.CropId);
                if (getCropRequirement?.CropRequirement == null || !getCropRequirement.CropRequirement.Any())
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Không tìm thấy cây trồng yêu cầu.");
                }
                await _unitOfWork.scheduleRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<ScheduleResponseView>(schedule);

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }
        //public async Task<ResponseDTO> AddFarmActivityToSchedule(long scheduleId, long farmActivities)
        //{
        //    try
        //    {
        //        // Kiểm tra quyền
        //        var currentUser = await _jwtUtils.GetCurrentUserAsync();
        //        if (currentUser == null || currentUser.Role != Roles.Manager)
        //        {
        //            return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
        //        }
        //        // Tìm schedule
        //        var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(scheduleId);
        //        if (schedule == null)
        //        {
        //            return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch  không tồn tại.");
        //        }
        //        else
        //        {

        //            var farmActivity = await _farmActivityRepository.GetByIdAsync(farmActivities);
        //            if (farmActivity == null)
        //            {
        //                return new ResponseDTO(Const.FAIL_CREATE_CODE, $"Không tìm thấy hoạt động nông trại với ID: {farmActivities}");
        //            }
        //            // Kiểm tra tính hợp lệ của schedule với farm activity
        //            var (isValid, validationMessage) = ValidateScheduleRequest(schedule, farmActivity);
        //            if (!isValid)
        //            {
        //                return new ResponseDTO(Const.FAIL_CREATE_CODE, validationMessage);
        //            }

        //            await _unitOfWork.scheduleRepository.AddAsync(schedule);
        //            await _unitOfWork.SaveChangesAsync();
        //        }
        //        // Map sang response view
        //        var result = _mapper.Map<ScheduleResponseView>(schedule);
        //        return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
        //    }
        //}
        public async Task<UpdateTodayResponse> UpdateTodayAsync(long scheduleId, UpdateTodayRequest request)
        {
            var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
            if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.");
            }

            var schedule = await _unitOfWork.scheduleRepository.GetByIdWithCropRequirementsAsync(scheduleId, getCurrentUser.AccountId);

            if (schedule == null)
                throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.");

            if (schedule.StartDate == null)
                throw new InvalidOperationException("Schedule chưa có StartDate.");

            // Xác định toDay
            var today = request?.CustomToday ?? DateOnly.FromDateTime(DateTime.Now);

            if (today < schedule.StartDate.Value)
                throw new InvalidOperationException("Ngày hiện tại không thể nhỏ hơn StartDate.");

            schedule.toDay = today;

            // Tính số ngày đã trôi qua
            var daysSinceStart = today.DayNumber - schedule.StartDate.Value.DayNumber;

            // Lấy danh sách yêu cầu theo cây trồng, chỉ lấy các record active và sort theo EstimatedDate
            var requirements = await _unitOfWork.cropRequirementRepository.GetActiveRequirementsOrderedAsync(schedule.CropId);

            if (!requirements.Any())
                throw new InvalidDataException("Cây trồng này chưa có yêu cầu chăm sóc nào.");

            // Tính giai đoạn hiện tại dựa trên số ngày đã trôi qua (daysSinceStart)
            CropRequirement? currentReq = null;
            CropRequirement? nextReq = null;

            int cumulativeDays = 0;

            foreach (var req in requirements)
            {
                if (req.EstimatedDate.HasValue && req.EstimatedDate.Value > 0)
                {
                    int stageEndDay = cumulativeDays + req.EstimatedDate.Value;

                    // Nếu ngày hiện tại nằm trong giai đoạn này (bao gồm ngày cuối)
                    if (daysSinceStart < stageEndDay || (daysSinceStart == stageEndDay && req == requirements.Last()))
                    {
                        currentReq = req;
                        break;
                    }

                    cumulativeDays = stageEndDay; // Cập nhật ngày kết thúc giai đoạn hiện tại
                }
            }

            // Nếu đã vượt quá tổng ngày → lấy giai đoạn cuối cùng
            currentReq ??= requirements.LastOrDefault();

            // Tìm giai đoạn tiếp theo
            var currentIndex = currentReq != null ? requirements.IndexOf(currentReq) : -1;
            if (currentIndex >= 0 && currentIndex < requirements.Count - 1)
            {
                nextReq = requirements[currentIndex + 1];
            }

            // Gán stage hiện tại
            schedule.currentPlantStage = currentReq?.PlantStage ?? PlantStage.Preparation;

            // Cập nhật UpdatedAt
            schedule.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);

            await _unitOfWork.SaveChangesAsync();

            // Tính ngày còn lại đến stage tiếp theo
            int? daysToNextStage = null;
            if (nextReq != null)
                daysToNextStage = nextReq.EstimatedDate - daysSinceStart;

            var response = (schedule, today, daysSinceStart, currentReq, nextReq, daysToNextStage);

            return _mapper.Map<UpdateTodayResponse>(response);
        }

        public async Task<ResponseDTO> UpdateSchedulesAsync(long scheduleId, ScheduleRequest request)
        {
            try
            {
                // Gọi hàm validate
                var (isValid, errorMessage) = await ValidateScheduleRequestAsync(request);

                if (!isValid)
                    return new ResponseDTO(Const.ERROR_EXCEPTION, errorMessage);

                // Kiểm tra quyền
                var currentUser = await _jwtUtils.GetCurrentUserAsync();
                if (currentUser == null || currentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }

                // Tìm schedule
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(scheduleId);
                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch  không tồn tại.");
                }

                // Map dữ liệu mới vào schedule cũ
                var updatedSchedule = _mapper.Map(request, schedule);
                updatedSchedule.UpdatedAt = _currentTime.GetCurrentTime();

                var today = _currentTime.GetCurrentTime(); // Ngày hiện tại 


                await _unitOfWork.scheduleRepository.UpdateAsync(updatedSchedule);
                await _unitOfWork.SaveChangesAsync();

                // Lấy thông tin staff và manager
                var manager = await _account.GetByIdAsync(updatedSchedule.ManagerId);

                // Map sang response view
                var result = _mapper.Map<ScheduleResponseView>(updatedSchedule);
                result.Manager = manager != null
                    ? _mapper.Map<Infrastructure.ViewModel.Response.AccountResponse.ViewAccount>(manager)
                    : null;

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }


        public async Task<ResponseDTO> GetAllSchedulesAsync(int pageIndex, int pageSize)
        {
            try
            {
                var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
                if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }

                var list = await _unitOfWork.scheduleRepository.GetAllAsync();

                var schedulesView = list.Select(schedule =>
                {
                    // map Schedule trước
                    var scheduleView = _mapper.Map<ScheduleResponseView>(schedule);

                    // map tay FarmActivity + Staff
                    scheduleView.farmActivityView = schedule.FarmActivities?
                        .Select(fa => new FarmActivityView
                        {
                            FarmActivitiesId = fa.FarmActivitiesId,
                            ActivityType = fa.ActivityType?.ToString(),
                            StartDate = fa.StartDate?.ToString("dd/MM/yyyy"),
                            EndDate = fa.EndDate?.ToString("dd/MM/yyyy"),
                            Status = fa.Status?.ToString(),

                            //StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                            //StaffEmail = fa.AssignedToNavigation?.Email,
                            //StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                            //StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
                        })
                        .ToList();

                    return scheduleView;
                }).ToList();

                var safePageIndex = Math.Max(pageIndex, 1);

                var result = new Pagination<ScheduleResponseView>
                {
                    TotalItemCount = schedulesView.Count(),
                    PageSize = pageSize,
                    PageIndex = safePageIndex,
                    Items = schedulesView
                        .Skip((safePageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList()
                };

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }


        public async Task<ResponseDTO> ScheduleByIdAsync(long scheduleId)
        {
            try
            {
                // Kiểm tra quyền
                var currentUser = await _jwtUtils.GetCurrentUserAsync();
                if (currentUser == null || currentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }

                // Lấy schedule
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(scheduleId);
                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch không tồn tại.");
                }

                // Map Schedule trước
                var scheduleView = _mapper.Map<ScheduleResponseView>(schedule);

                // ===== MAP TAY FarmActivity + Staff (GIỐNG HÀM LIST) =====
                scheduleView.farmActivityView = schedule.FarmActivities?
                    .Select(fa => new FarmActivityView
                    {
                        FarmActivitiesId = fa.FarmActivitiesId,
                        ActivityType = fa.ActivityType?.ToString(),
                        StartDate = fa.StartDate?.ToString("dd/MM/yyyy"),
                        EndDate = fa.EndDate?.ToString("dd/MM/yyyy"),
                        Status = fa.Status?.ToString(),

                        //StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                        //StaffEmail = fa.AssignedToNavigation?.Email,
                        //StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                        //StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
                    })
                    .ToList();

                return new ResponseDTO(
                    Const.SUCCESS_READ_CODE,
                    Const.SUCCESS_READ_MSG,
                    scheduleView
                );
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }


        public async Task<ResponseDTO> AssignTask(long scheduleID, long staffId)
        {
            try
            {
                var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
                if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }

                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(scheduleID);
                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch  không tồn tại.");
                }

                var staffInfo = await _account.GetByIdAsync(staffId);
                if (staffInfo == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Nhân viên không tồn tại.");
                }
                var checkStaff = await _unitOfWork.scheduleRepository.GetScheduleByStaffIdAsync(staffId, 0);
                {
                    if (checkStaff.Any())
                    {
                        return new ResponseDTO(Const.FAIL_CREATE_CODE, "Nhân viên đã được phân công ở một lịch khác!");
                    }

                    schedule.UpdatedAt = _currentTime.GetCurrentTime();

                    await _unitOfWork.scheduleRepository.UpdateAsync(schedule);
                    await _unitOfWork.SaveChangesAsync();

                    // In ra kết quả
                    var result = _mapper.Map<ScheduleResponseView>(schedule);
                    var manager = await _account.GetByIdAsync(schedule.ManagerId);

                    return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);
                }
            }

            catch
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Lỗi khi phân công nhiệm vụ.");
            }
        }

        public async Task<ResponseDTO> UpdateActivities(long scheduleId, long[]? activityId)
        {
            try
            {
                // Kiểm tra quyền
                var currentUser = await _jwtUtils.GetCurrentUserAsync();
                if (currentUser == null || currentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }

                // Tìm schedule
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(scheduleId);
                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch  không tồn tại.");
                }

                // Cập nhật activity
                //schedule.FarmActivitiesId = activityId;
                //schedule.UpdatedAt = _currentTime.GetCurrentTime();
                //var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(schedule.FarmActivitiesId);

                //if (!ValidateScheduleRequest(schedule, farmActivity).Item1)
                //{
                //    return new ResponseDTO(Const.FAIL_CREATE_CODE, ValidateScheduleRequest(schedule, farmActivity).Item2);
                //}
                // Lưu thay đổi
                _unitOfWork.scheduleRepository.Update(schedule);
                await _unitOfWork.SaveChangesAsync();

                // Map sang response view
                var result = _mapper.Map<ScheduleResponseView>(schedule);

                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }


        public async Task<ResponseDTO> ChangeScheduleStatusById(long scheduleId)
        {
            try
            {
                // 1. Kiểm tra quyền Manager
                var currentUser = await _jwtUtils.GetCurrentUserAsync();
                if (currentUser == null || currentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Bạn không có quyền thay đổi trạng thái lịch trình.");
                }

                // 2. Tìm Schedule
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(scheduleId);
                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch trình không tồn tại.");
                }

                // 3. Xác định trạng thái mới (toggle)
                Status newStatus = schedule.Status == Status.ACTIVE
                    ? Status.DEACTIVATED
                    : Status.ACTIVE;

                // 4. Validate chỉ khi **chuyển sang ACTIVE**
                var today = DateOnly.FromDateTime(_currentTime.GetCurrentTime().ToDateTime(TimeOnly.MinValue));

                if (newStatus == Status.ACTIVE)
                {
                    // Kiểm tra chồng chéo thời gian với các Schedule ACTIVE hiện tại (khác với lịch đang toggle)
                    bool hasOverlapping = await _unitOfWork.scheduleRepository.HasOverlappingActiveScheduleAsync(
                        startDate: schedule.StartDate.Value,
                        endDate: schedule.EndDate.Value);

                    if (hasOverlapping)
                    {
                        return new ResponseDTO(Const.ERROR_EXCEPTION,
                            $"Không thể kích hoạt lịch trình vì thời gian từ {schedule.StartDate:dd/MM/yyyy} đến {schedule.EndDate:dd/MM/yyyy} đang chồng chéo với lịch trình đang hoạt động khác.");
                    }

                    // Không cho active lại lịch đã bắt đầu trong quá khứ
                    if (schedule.StartDate < today)
                    {
                        return new ResponseDTO(Const.ERROR_EXCEPTION, "Không thể kích hoạt lại lịch trình đã bắt đầu trong quá khứ.");
                    }

                    // (Tùy chọn) Không cho active lịch đã hết hạn
                    if (schedule.EndDate < today)
                    {
                        return new ResponseDTO(Const.ERROR_EXCEPTION, "Không thể kích hoạt lịch trình đã kết thúc trong quá khứ.");
                    }
                }
                // Không cần validate khi chuyển sang DEACTIVATED (luôn cho phép)

                // 5. Cập nhật trạng thái mới
                schedule.Status = newStatus;
                _unitOfWork.scheduleRepository.Update(schedule);
                await _unitOfWork.SaveChangesAsync();

                // 6. Trả kết quả
                var result = _mapper.Map<ScheduleResponseView>(schedule);
                string actionMsg = newStatus == Status.ACTIVE ? "kích hoạt" : "tạm dừng";
                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, $"Đã {actionMsg} lịch trình thành công.", result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Đã xảy ra lỗi khi thay đổi trạng thái lịch trình.");
            }
        }

        public async Task<ResponseDTO> ScheduleStaffView(int month)
        {
            try
            {
                // Kiểm tra quyền
                var currentUser = await _jwtUtils.GetCurrentUserAsync();
                if (currentUser == null || currentUser.Role != Roles.Staff)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }

                // Lấy danh sách schedule theo staff + tháng
                var list = await _unitOfWork.scheduleRepository
                    .GetByStaffIdAsync(currentUser.AccountId, month);

                if (list == null || !list.Any())
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Không tìm thấy lịch.");
                }

                // ===== MAP TAY SCHEDULE + FARM ACTIVITY + STAFF =====
                var schedulesView = list.Select(schedule =>
                {
                    // Map Schedule trước
                    var scheduleView = _mapper.Map<ScheduleResponseView>(schedule);

                    // Chỉ lấy FarmActivity của staff đang đăng nhập
                    scheduleView.farmActivityView = schedule.FarmActivities?
                        //.Where(fa =>
                        //    fa.AssignedToNavigation != null &&
                        //    fa.AssignedToNavigation.AccountId == currentUser.AccountId
                        //)
                        .Select(fa => new FarmActivityView
                        {
                            FarmActivitiesId = fa.FarmActivitiesId,
                            ActivityType = fa.ActivityType?.ToString(),
                            StartDate = fa.StartDate?.ToString("dd/MM/yyyy"),
                            EndDate = fa.EndDate?.ToString("dd/MM/yyyy"),
                            Status = fa.Status?.ToString(),

                            //StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                            //StaffEmail = fa.AssignedToNavigation?.Email,
                            //StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                            //StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
                        })
                        .ToList();

                    return scheduleView;
                }).ToList();

                return new ResponseDTO(
                    Const.SUCCESS_READ_CODE,
                    Const.SUCCESS_READ_MSG,
                    schedulesView
                );
            }
            catch (Exception ex)
            {
                return new ResponseDTO(
                    Const.FAIL_READ_CODE,
                    $"Lỗi khi lấy lịch: {ex.Message}"
                );
            }
        }

    }
    #endregion
}
