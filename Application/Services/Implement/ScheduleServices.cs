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
using System.Drawing.Printing;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.CropRequirementResponse;
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
        public (bool, string) ValidateScheduleRequest(Schedule schedule, FarmActivity farmActivity)
        {
            // Kiểm tra ngày bắt đầu/kết thúc của schedule
            if (schedule.StartDate == null || schedule.EndDate == null)
            {
                return (false, "Ngày bắt đầu và ngày kết thúc của lịch không được để trống.");
            }
            if (schedule.StartDate > schedule.EndDate)
            {
                return (false, "Ngày bắt đầu phải trước ngày kết thúc.");
            }

            //if (farmActivity.StartDate == null || farmActivity.EndDate == null)
            //    if (farmActivity.StartDate == null || farmActivity.EndDate == null)
            //    {
            //        return (false, "Ngày bắt đầu và kết thúc của hoạt động không được để trống.");
            //    }
            //if (farmActivity.StartDate > farmActivity.EndDate)
            //    if (farmActivity.StartDate > farmActivity.EndDate)
            //    {
            //        return (false, "Ngày bắt đầu của hoạt động phải trước ngày kết thúc.");
            //    }

            //// Kiểm tra hoạt động nằm trong khoảng của schedule
            //if (farmActivity.StartDate < schedule.StartDate
            //    || farmActivity.EndDate > schedule.EndDate)
            //    if (farmActivity.StartDate < schedule.StartDate
            //        || farmActivity.EndDate > schedule.EndDate)
            //    {
            //        return (false, $"Hoạt động từ {farmActivity.StartDate:dd/MM/yyyy} đến {farmActivity.EndDate:dd/MM/yyyy} nằm ngoài khoảng thời gian của lịch.");
            //        //return (false, $"Hoạt động từ {farmActivity.StartDate:dd/MM/yyyy} đến {farmActivity.EndDate:dd/MM/yyyy} nằm ngoài khoảng thời gian của lịch.");
            //    }
            //// Kiểm tra ngày bắt đầu của hoạt động phải ít nhất là hôm nay
            //if (farmActivity.StartDate < DateOnly.FromDateTime(DateTime.Today))
            //{
            //    return (false, "Ngày bắt đầu của hoạt động phải từ hôm nay trở đi.");
            //}

            // Vì mỗi schedule chỉ có 1 farm activity, không cần check trùng ngày nhiều activity
            return (true, string.Empty);
        }

        #region Schedule mới
        public async Task<ResponseDTO> CreateSchedulesAsync(ScheduleRequest request)
        {
            try
            {
                // Kiểm tra quyền
                var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
                if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }
                // Tạo schedule mới
                var schedule = _mapper.Map<Schedule>(request);
                schedule.ManagerId = getCurrentUser.AccountId;
                schedule.CreatedAt = _currentTime.GetCurrentTime();
                schedule.currentPlantStage = PlantStage.Seedling;
                // Kiểm tra yêu cầu cây trồng
                var getCropRequirement = await _cropRepository.GetByIdAsync(request.CropId);
                if (getCropRequirement?.CropRequirement == null || !getCropRequirement.CropRequirement.Any())
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Không tìm thấy cây trồng yêu cầu.");
                }
                var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(request.FarmActivitiesId);
                if (!ValidateScheduleRequest(schedule, farmActivity).Item1)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, ValidateScheduleRequest(schedule, farmActivity).Item2);
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

            // Tìm stage hiện tại: stage có EstimatedDate lớn nhất mà daysSinceStart <= EstimatedDate
            CropRequirement? currentReq = null;
            CropRequirement? nextReq = null;

            foreach (var req in requirements)
            {
                if (daysSinceStart <= req.EstimatedDate)
                {
                    currentReq = req;
                    break;
                }
            }

            // Nếu đã vượt qua tất cả các stage → lấy stage cuối cùng
            currentReq ??= requirements.Last();

            // Xác định vị trí của giai đoạn hiện tại trong chuỗi các giai đoạn
            var currentIndex = requirements.IndexOf(currentReq);
            //Lấy chính xác giai đoạn kế tiếp
            if (currentIndex < requirements.Count - 1)
                nextReq = requirements[currentIndex + 1];

            // Gán stage hiện tại
            schedule.currentPlantStage = currentReq.PlantStage ?? PlantStage.Germination;

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
                var currentActivityId = schedule.FarmActivitiesId;
                var requestedActivityId = request.FarmActivitiesId;

                if (currentActivityId != requestedActivityId && currentActivityId > 0)
                {
                    var currentActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(currentActivityId);
                    if (currentActivity == null)
                    {
                        return new ResponseDTO(Const.FAIL_CREATE_CODE, "Hoạt động hiện tại không tồn tại hoặc đã bị xóa.");
                    }

                    // Thay ActivityStatus.COMPLETED bằng enum thực tế của bạn (ví dụ: Completed, Done, Finished)
                    if (currentActivity.Status != FarmActivityStatus.COMPLETED)
                    {
                        return new ResponseDTO(Const.FAIL_CREATE_CODE,
                            "Không thể cập nhật sang hoạt động mới vì hoạt động hiện tại chưa được hoàn thành.");
                    }
                }
                // Map dữ liệu mới vào schedule cũ
                var updatedSchedule = _mapper.Map(request, schedule);
                updatedSchedule.UpdatedAt = _currentTime.GetCurrentTime();
                //if (!ValidateScheduleRequest(schedule).Item1)
                //{
                //    return new ResponseDTO(Const.FAIL_CREATE_CODE, ValidateScheduleRequest(schedule).Item2);
                //}
                //var checkStaff = await _unitOfWork.scheduleRepository.GetByStaffIdAsync(request.StaffId, 0);
                //if (checkStaff != null && checkStaff.Any())
                //{
                //    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Nhân viên đã được phân công ở một lịch khác!");
                //}

                // Validate request cơ bản (sử dụng updatedSchedule để có dữ liệu mới nhất)
                var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(request.FarmActivitiesId);

                if (!ValidateScheduleRequest(updatedSchedule, farmActivity).Item1)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, ValidateScheduleRequest(updatedSchedule, farmActivity).Item2);
                }

                // Lấy FarmActivity mới từ request
                var newActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(request.FarmActivitiesId);
                if (newActivity == null)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Hoạt động không tồn tại.");
                }

                var today = _currentTime.GetCurrentTime(); // Ngày hiện tại 

                // Validate thời gian activity không trong quá khứ
                if (newActivity.StartDate.HasValue && newActivity.StartDate.Value < today.AddDays(-1)) // Cho phép hôm qua
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Ngày bắt đầu của hoạt động không được trước hôm qua.");
                }
                if (newActivity.EndDate.HasValue && newActivity.EndDate.Value < today)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Ngày kết thúc của hoạt động không được trong quá khứ.");
                }

                // Validate thời gian activity phải nằm trong khung của Schedule
                if (updatedSchedule.StartDate.HasValue && newActivity.StartDate.HasValue &&
                    newActivity.StartDate.Value < updatedSchedule.StartDate.Value)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Ngày bắt đầu của hoạt động phải sau hoặc bằng ngày bắt đầu của lịch.");
                }
                if (updatedSchedule.EndDate.HasValue && newActivity.EndDate.HasValue &&
                    newActivity.EndDate.Value > updatedSchedule.EndDate.Value)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Ngày kết thúc của hoạt động phải trước hoặc bằng ngày kết thúc của lịch.");
                }

                // Loại bỏ chính schedule đang được update (vì nó là cùng lịch, được phép overlap với chính nó)
                //var otherSchedules = allSchedulesOfStaff
                //    .Where(s => s.ScheduleId != scheduleId)
                //    .ToList();

                //// Kiểm tra xem có lịch KHÁC nào đang ACTIVE và bị overlap thời gian với dữ liệu mới không
                //var hasOverlapWithActiveSchedule = otherSchedules.Any(s =>
                //    s.Status == Status.ACTIVE &&
                //    s.StartDate.HasValue &&
                //    s.EndDate.HasValue &&
                //    updatedSchedule.StartDate.HasValue &&
                //    updatedSchedule.EndDate.HasValue &&
                //    updatedSchedule.StartDate < s.EndDate &&
                //    updatedSchedule.EndDate > s.StartDate
                //);

                //if (hasOverlapWithActiveSchedule)
                //{
                //    return new ResponseDTO(Const.FAIL_CREATE_CODE,
                //        "Thời gian cập nhật bị chồng lấn với một lịch đang active khác của nhân viên này!");
                //}

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
                var schedules = _mapper.Map<List<ScheduleResponseView>>(list);

                //In ra kết quả

                var safePageIndex = Math.Max(pageIndex, 1); // Đảm bảo pageIndex >= 1

                var result = new Pagination<ScheduleResponseView>
                {
                    TotalItemCount = schedules.Count,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = schedules.Skip((safePageIndex - 1) * pageSize).Take(pageSize).ToList()
                };



                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_CREATE_MSG, result);


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

                // Tìm schedule
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(scheduleId);
                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch không tồn tại.");
                }

                // Map sang response view
                var result = _mapper.Map<ScheduleResponseView>(schedule);
                // Lấy thông tin manager
                var manager = await _account.GetByIdAsync(schedule.ManagerId);

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
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

        public async Task<ResponseDTO> UpdateActivities(long scheduleId, long activityId)
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
                schedule.FarmActivitiesId = activityId;
                schedule.UpdatedAt = _currentTime.GetCurrentTime();
                var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(schedule.FarmActivitiesId);

                if (!ValidateScheduleRequest(schedule, farmActivity).Item1)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, ValidateScheduleRequest(schedule, farmActivity).Item2);
                }
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


        public async Task<ResponseDTO> ChangeScheduleStatusById(long scheduleId, string status)
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

                // Kiểm tra trạng thái hiện tại
                schedule.Status = (schedule.Status == Status.ACTIVE)
                    ? Status.DEACTIVATED
                    : Status.ACTIVE;

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

                // Lấy danh sách lịch  theo staff và tháng
                var list = await _unitOfWork.scheduleRepository.GetByStaffIdAsync(currentUser.AccountId, month);
                if (list == null || !list.Any())
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Không tìm thấy lịch.");
                }

                // Map sang response view
                var schedules = _mapper.Map<List<ScheduleResponseView>>(list);



                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, schedules);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, $"Lỗi khi lấy lịch : {ex.Message}");
            }
        }
    }
    #endregion
}
