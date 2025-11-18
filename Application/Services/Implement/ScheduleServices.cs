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
        public async Task<(bool, string)> ValidateScheduleDate(Schedule schedule)
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
            long farmAcitivityId = schedule.FarmActivitiesId;
            if (farmAcitivityId != 0)
            {
                var activity = await _farmActivityRepository.GetByIdAsync((long)farmAcitivityId);
                if (activity != null)
                {
                    DateOnly? activityStartDate = activity.StartDate;
                    DateOnly? activityEndDate = activity.EndDate;
                    if (activityStartDate == null || activityEndDate == null)
                    {
                        return (false, "Hoạt động chưa có ngày bắt đầu hoặc ngày kết thúc.");
                    }

                    // Chỉ kiểm tra các mốc thời gian liên quan trực tiếp đến hoạt động
                    if (activityStartDate < startDate || activityEndDate > endDate)
                    {
                        return (false, "Ngày gieo trồng phải nằm trong khoảng thời gian của hoạt động.");
                    }

                    //if (harvestDate < activityStartDate || harvestDate > activityEndDate)
                    //{
                    //    return (false, "Ngày thu hoạch phải nằm trong khoảng thời gian của hoạt động.");
                    //}
                }
            }
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

                var validate = ValidateScheduleDate(schedule);
                if (schedule.StartDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, "Ngày tạo nằm trong quá khứ.");
                }

                if (schedule.PlantingDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, "Ngày gieo trồng nằm trong quá khứ.");
                }

                if (validate.Result.Item1 == false)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, validate.Result.Item2);
                }
                await _unitOfWork.scheduleRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
                //In ra kết quả
                var staffInfo = await _account.GetByIdAsync(request.StaffId);
                var result = _mapper.Map<ScheduleResponseView>(schedule);
                result.ManagerName = getCurrentUser.AccountProfile.Fullname;
                result.StaffName = staffInfo != null ? staffInfo.AccountProfile.Fullname : null;

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);


            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateSchedulesAsync(long ScheduleId, ScheduleRequest request)
        {
            try
            {
                var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
                if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(ScheduleId);
                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch trình không tồn tại.");
                }
                var updatedSchedule = _mapper.Map<ScheduleRequest, Schedule>(request, schedule);
                updatedSchedule.UpdatedAt = _currentTime.GetCurrentTime();
                _unitOfWork.scheduleRepository.Update(updatedSchedule);
                await _unitOfWork.SaveChangesAsync();
                //In ra kết quả
                var staffInfo = await _account.GetByIdAsync(request.StaffId);
                var result = _mapper.Map<ScheduleResponseView>(updatedSchedule);

                var getCreateUser = await _account.GetByIdAsync(updatedSchedule.ManagerId);

                result.ManagerName = getCreateUser != null ? getCreateUser.AccountProfile.Fullname : null;
                result.StaffName = staffInfo != null ? staffInfo.AccountProfile.Fullname : null;
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

        public async Task<ResponseDTO> ScheduleByIdAsync(long ScheduleId)
        {
            try
            {
                var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
                if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }

                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(ScheduleId);
                var result = _mapper.Map<ScheduleResponseView>(schedule);
                //In ra kết quả

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
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch trình không tồn tại.");
                }
                schedule.AssignedTo = staffId;
                schedule.UpdatedAt = _currentTime.GetCurrentTime();
                _unitOfWork.scheduleRepository.Update(schedule);
                await _unitOfWork.SaveChangesAsync();
                //In ra kết quả
                var result = _mapper.Map<ScheduleResponseView>(schedule);
                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);

            }
            catch
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Lỗi khi phân công nhiệm vụ.");
            }
        }

        public async Task<ResponseDTO> UpdateActivities(long ScheduleId, long activityId)
        {
            try
            {
                var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
                if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(ScheduleId);
                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch trình không tồn tại.");
                }
                schedule.FarmActivitiesId = activityId;
                schedule.UpdatedAt = _currentTime.GetCurrentTime();

                //validate date của schedule sau khi update activities
                var validate = ValidateScheduleDate(schedule);
                if (validate.Result.Item1 == false)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, validate.Result.Item2);
                }

                _unitOfWork.scheduleRepository.Update(schedule);
                await _unitOfWork.SaveChangesAsync();
                //In ra kết quả
                var result = _mapper.Map<ScheduleResponseView>(schedule);

                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);

            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }

        public async Task<ResponseDTO> ChangeScheduleStatusById(long ScheduleId, string status)
        {
            try
            {
                var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
                if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(ScheduleId);
                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Lịch trình không tồn tại.");
                }
                if (schedule.Status.ToString() == status)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Trạng thái không thay đổi.");
                }
                else
                {
                    schedule.Status = Enum.Parse<Status>(status);
                    schedule.UpdatedAt = _currentTime.GetCurrentTime();
                }

                //validate date của schedule sau khi update activities
                var validate = ValidateScheduleDate(schedule);
                if (validate.Result.Item1 == false)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, validate.Result.Item2);
                }

                _unitOfWork.scheduleRepository.Update(schedule);
                await _unitOfWork.SaveChangesAsync();
                //In ra kết quả
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
                var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
                if (getCurrentUser == null || getCurrentUser.Role != Roles.Staff)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }
                var list = await _unitOfWork.scheduleRepository.GetByStaffIdAsync(getCurrentUser.AccountId, month);
                var schedules = _mapper.Map<List<ScheduleResponseView>>(list);
                //In ra kết quả
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, schedules);

            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }
    }
    #endregion
}
