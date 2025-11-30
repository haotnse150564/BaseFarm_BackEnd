using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using AutoMapper;
using Azure.Core;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
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

        #region Validation date Schedule
        public virtual async Task<(bool, string)> ValidateScheduleDate(Schedule schedule)
        {
            int result = 0;
            string message = "";
            DateOnly? startDate = schedule.StartDate, endDate = schedule.EndDate, plantingDate = schedule.PlantingDate, harvestDate = schedule.HarvestDate;
            #region check chi tiết
            //các trường hợp cần Validate
            //Schedule chưa có activity
            //Check ngày có phải ở quá khứ hay không 
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (startDate == null || endDate == null || plantingDate == null || harvestDate == null)
            {
                return (false, "Một hoặc nhiều ngày chưa được thiết lập.");
            }

            if (endDate.Value < today)
            {
                return (false, "Ngày kết thúc nằm trong quá khứ.");
            }

            if (harvestDate.Value < today)
            {
                return (false, "Ngày thu hoạch nằm trong quá khứ.");
            }
            //kiểm tra logic ngày trước - sau
            if (plantingDate > harvestDate)
            {
                return (false, "Ngày gieo trồng phải trước ngày thu hoạch.");
            }

            if (startDate > endDate)
            {
                return (false, "Ngày tạo phải trước ngày kết thúc.");
            }
            if (harvestDate > endDate)
            {
                return (false, "Ngày thu hoạch phải trước ngày kết thúc.");
            }
            //logic về khoảng thời gian trong ngày 
            var dates = new[] { startDate.Value, endDate.Value, plantingDate.Value, harvestDate.Value };
            if (dates.Distinct().Count() < dates.Length)
                return (false, "Các ngày không được trùng nhau.");
            if (startDate == endDate)
                return (false, "Ngày tạo và ngày kết thúc không được trùng nhau.");

            if (startDate == plantingDate)
                return (false, "Ngày tạo và ngày gieo trồng không được trùng nhau.");

            if (startDate == harvestDate)
                return (false, "Ngày tạo và ngày thu hoạch không được trùng nhau.");

            if (endDate == plantingDate)
                return (false, "Ngày kết thúc và ngày gieo trồng không được trùng nhau.");

            if (endDate == harvestDate)
                return (false, "Ngày kết thúc và ngày thu hoạch không được trùng nhau.");

            if (plantingDate == harvestDate)
                return (false, "Ngày gieo trồng và ngày thu hoạch không được trùng nhau.");

            #endregion
            //Schedule có activity
            #region Activity check Date
            //long farmAcitivityId = schedule.FarmActivitiesId;
            //if (farmAcitivityId != 0)
            //{
            //    var activity = await _farmActivityRepository.GetByIdAsync((long)farmAcitivityId);
            //    if (activity != null)
            //    {
            //        DateOnly? activityStartDate = activity.StartDate;
            //        DateOnly? activityEndDate = activity.EndDate;
            //        if (activityStartDate == null || activityEndDate == null)
            //        {
            //            return (false, "Hoạt động chưa có ngày bắt đầu hoặc ngày kết thúc.");
            //        }

            //        // Chỉ kiểm tra các mốc thời gian liên quan trực tiếp đến hoạt động
            //        if (activityStartDate < startDate || activityEndDate > endDate)
            //        {
            //            return (false, "Ngày gieo trồng phải nằm trong khoảng thời gian của hoạt động.");
            //        }

            //        //if (harvestDate < activityStartDate || harvestDate > activityEndDate)
            //        //{
            //        //    return (false, "Ngày thu hoạch phải nằm trong khoảng thời gian của hoạt động.");
            //        //}
            //    }
           // }
            #endregion

            if (result == 0) return (true, "");
            return (false, message);
        }
        #endregion
        #region Schedule mới
        public async Task<ResponseDTO> CreateSchedulesAsync(ScheduleRequest request)
        {
            try
            {
                var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
                if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }

                var schedule = _mapper.Map<Schedule>(request);
                schedule.ManagerId = getCurrentUser.AccountId;
                schedule.CreatedAt = _currentTime.GetCurrentTime();

                var getCropRequirement = await _cropRepository.GetByIdAsync(request.CropId);
                if (getCropRequirement?.CropRequirement == null || !getCropRequirement.CropRequirement.Any())
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Không tìm thấy yêu cầu cây trồng.");
                }

                if (!schedule.PlantingDate.HasValue)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Ngày gieo trồng chưa được nhập.");
                }

                var requirement = getCropRequirement.CropRequirement.First();
                schedule.HarvestDate = schedule.PlantingDate.Value.AddDays((int)requirement.EstimatedDate);

                var validate = await ValidateScheduleDate(schedule);
                if (!validate.Item1)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, validate.Item2);
                }
                await _unitOfWork.scheduleRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                var staffInfo = await _account.GetByIdAsync(request.StaffId);
                var result = _mapper.Map<ScheduleResponseView>(schedule);
                result.ManagerName = getCurrentUser.AccountProfile?.Fullname;
                result.StaffName = staffInfo?.AccountProfile?.Fullname;

                // Lọc crop requirement theo PlanStage
                if (result.farmActivityView != null && result.CropRequirement != null && result.CropRequirement.Any())
                {
                    var matchedRequirement = result.CropRequirement
                        .FirstOrDefault(r => r.PlantStage == result.farmActivityView.plantStage);

                    result.CropRequirement = matchedRequirement != null
                        ? new List<CropRequirementView> { matchedRequirement }
                        : new List<CropRequirementView>();
                }

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);


            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
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

                // Lọc crop requirement theo PlanStage
                if (result.farmActivityView != null && result.CropRequirement != null && result.CropRequirement.Any())
                {
                    var matchedRequirement = result.CropRequirement
                        .FirstOrDefault(r => r.PlantStage == result.farmActivityView.plantStage);

                    result.CropRequirement = matchedRequirement != null
                        ? new List<CropRequirementView> { matchedRequirement }
                        : new List<CropRequirementView>();
                }

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

                    if (item.farmActivityView != null && item.CropRequirement != null && item.CropRequirement.Any())
                    {
                        var matchedRequirement = item.CropRequirement
                            .FirstOrDefault(r => r.PlantStage == item.farmActivityView.plantStage);

                        // Nếu tìm thấy thì chỉ giữ lại requirement đó
                        if (matchedRequirement != null)
                        {
                            item.CropRequirement = new List<CropRequirementView> { matchedRequirement };
                        }
                        else
                        {
                            // Nếu không có cái nào khớp thì clear list
                            item.CropRequirement = new List<CropRequirementView>();
                        }
                    }

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

                // Lọc crop requirement theo PlanStage
                if (result.farmActivityView != null && result.CropRequirement != null && result.CropRequirement.Any())
                {
                    var matchedRequirement = result.CropRequirement
                        .FirstOrDefault(r => r.PlantStage == result.farmActivityView.plantStage);

                    result.CropRequirement = matchedRequirement != null
                        ? new List<CropRequirementView> { matchedRequirement }
                        : new List<CropRequirementView>();
                }

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

                schedule.AssignedTo = staffId;
                schedule.UpdatedAt = _currentTime.GetCurrentTime();

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

                // Validate ngày
                var validate = ValidateScheduleDate(schedule);
                if (validate == null || validate.Result.Item1 == false)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG,
                                           validate?.Result.Item2 ?? "Lỗi kiểm tra ngày.");
                }

                // Lưu thay đổi
                _unitOfWork.scheduleRepository.Update(schedule);
                await _unitOfWork.SaveChangesAsync();

                // Map sang response view
                var result = _mapper.Map<ScheduleResponseView>(schedule);

                // Lọc crop requirement theo PlanStage
                if (result.farmActivityView != null && result.CropRequirement != null && result.CropRequirement.Any())
                {
                    var matchedRequirement = result.CropRequirement
                        .FirstOrDefault(r => r.PlantStage == result.farmActivityView.plantStage);

                    result.CropRequirement = matchedRequirement != null
                        ? new List<CropRequirementView> { matchedRequirement }
                        : new List<CropRequirementView>();
                }

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

                // Validate ngày
                var validate = ValidateScheduleDate(schedule);
                if (validate == null || validate.Result.Item1 == false)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG,
                                           validate?.Result.Item2 ?? "Lỗi kiểm tra ngày.");
                }

                // Lưu thay đổi
                _unitOfWork.scheduleRepository.Update(schedule);
                await _unitOfWork.SaveChangesAsync();

                // Map sang response view
                var result = _mapper.Map<ScheduleResponseView>(schedule);

                // Lọc crop requirement theo PlanStage
                if (result.farmActivityView != null && result.CropRequirement != null && result.CropRequirement.Any())
                {
                    var matchedRequirement = result.CropRequirement
                        .FirstOrDefault(r => r.PlantStage == result.farmActivityView.plantStage);

                    result.CropRequirement = matchedRequirement != null
                        ? new List<CropRequirementView> { matchedRequirement }
                        : new List<CropRequirementView>();
                }

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

                // Lọc crop requirement theo PlanStage cho từng schedule
                foreach (var item in schedules)
                {
                    if (item.farmActivityView != null && item.CropRequirement != null && item.CropRequirement.Any())
                    {
                        var matchedRequirement = item.CropRequirement
                            .FirstOrDefault(r => r.PlantStage == item.farmActivityView.plantStage);

                        item.CropRequirement = matchedRequirement != null
                            ? new List<CropRequirementView> { matchedRequirement }
                            : new List<CropRequirementView>();
                    }
                }

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
