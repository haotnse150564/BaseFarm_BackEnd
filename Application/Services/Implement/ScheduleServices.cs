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
        public (bool, string) ValidateScheduleRequest(Schedule schedule)
        {
            if (schedule.StartDate == null || schedule.EndDate == null)
            {
                return (false, "Ngày bắt đầu và ngày kết thúc không được để trống.");
            }
            if (schedule.StartDate > schedule.EndDate)
            {
                return (false, "Ngày bắt đầu phải trước ngày kết thúc.");
            }
            if (schedule.Quantity <= 0)
            {
                return (false, "Số lượng phải lớn hơn 0.");
            }
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
                if(!ValidateScheduleRequest(schedule).Item1)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, ValidateScheduleRequest(schedule).Item2);
                }
                var checkStaff = await _unitOfWork.scheduleRepository.GetByStaffIdAsync(request.StaffId, 0);
                if (checkStaff != null && checkStaff.Any())
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Nhân viên đã được phân công ở một lịch khác!");
                }
                await _unitOfWork.scheduleRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                var staffInfo = await _account.GetByIdAsync(request.StaffId);
                var result = _mapper.Map<ScheduleResponseView>(schedule);
                result.ManagerName = getCurrentUser.AccountProfile?.Fullname;
                result.StaffName = staffInfo?.AccountProfile?.Fullname;

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

                // Map dữ liệu mới vào schedule cũ
                var updatedSchedule = _mapper.Map(request, schedule);
                updatedSchedule.UpdatedAt = _currentTime.GetCurrentTime();
                if (!ValidateScheduleRequest(schedule).Item1)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, ValidateScheduleRequest(schedule).Item2);
                }
                var checkStaff = await _unitOfWork.scheduleRepository.GetByStaffIdAsync(request.StaffId, 0);
                if (checkStaff != null && checkStaff.Any())
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Nhân viên đã được phân công ở một lịch khác!");
                }
                await _unitOfWork.scheduleRepository.UpdateAsync(updatedSchedule);
                await _unitOfWork.SaveChangesAsync();

                // Lấy thông tin staff và manager
                var staffInfo = await _account.GetByIdAsync(request.StaffId);
                var manager = await _account.GetByIdAsync(updatedSchedule.ManagerId);

                // Map sang response view
                var result = _mapper.Map<ScheduleResponseView>(updatedSchedule);
                result.Manager = manager != null
                    ? _mapper.Map<Infrastructure.ViewModel.Response.AccountResponse.ViewAccount>(manager)
                    : null;
                result.ManagerName = manager?.AccountProfile?.Fullname;
                result.StaffName = staffInfo?.AccountProfile?.Fullname;

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
                foreach (var item in schedules)
                {
                    var manager = await _account.GetByIdAsync(item.ManagerId);
                    item.ManagerName = manager != null ? manager.AccountProfile.Fullname : null;
                }
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
                result.ManagerName = manager?.AccountProfile?.Fullname;

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
                var checkStaff = await _unitOfWork.scheduleRepository.GetByStaffIdAsync(schedule.AssignedTo, 0);
                if (checkStaff != null && checkStaff.Any())
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Nhân viên đã được phân công ở một lịch khác!");
                }
                schedule.AssignedTo = staffId;
                schedule.UpdatedAt = _currentTime.GetCurrentTime();
                if (!ValidateScheduleRequest(schedule).Item1)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, ValidateScheduleRequest(schedule).Item2);
                }
                await _unitOfWork.scheduleRepository.UpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                // In ra kết quả
                var result = _mapper.Map<ScheduleResponseView>(schedule);
                var manager = await _account.GetByIdAsync(schedule.ManagerId);
                result.ManagerName = manager?.AccountProfile?.Fullname;
                result.StaffName = staffInfo?.AccountProfile?.Fullname;

                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);
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
                if (!ValidateScheduleRequest(schedule).Item1)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, ValidateScheduleRequest(schedule).Item2);
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
                if (schedule.Status == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Trạng thái hiện tại chưa được thiết lập.");
                }

                if (schedule.Status.ToString() == status)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Trạng thái không thay đổi.");
                }

                // Parse trạng thái mới
                if (!Enum.TryParse<Status>(status, out var newStatus))
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Trạng thái không hợp lệ.");
                }

                // Cập nhật trạng thái
                schedule.Status = newStatus;
                schedule.UpdatedAt = _currentTime.GetCurrentTime();

                if (!ValidateScheduleRequest(schedule).Item1)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, ValidateScheduleRequest(schedule).Item2);
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
