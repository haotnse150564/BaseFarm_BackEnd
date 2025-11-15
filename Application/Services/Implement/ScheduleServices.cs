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
            DateOnly? createDate = schedule.CreatedAt, endDate = schedule.EndDate, plantingDate = schedule.PlantingDate, harvestDate = schedule.HarvestDate;
            #region check chi tiết
            //các trường hợp cần Validate
            //Schedule chưa có activity
            //Check ngày có phải ở quá khứ hay không 
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (createDate == null || endDate == null || plantingDate == null || harvestDate == null)
            {
                return (false, "Một hoặc nhiều ngày chưa được thiết lập.");
            }

            if (createDate.Value < today)
            {
                return (false, "Ngày tạo nằm trong quá khứ.");
            }

            if (endDate.Value < today)
            {
                return (false, "Ngày kết thúc nằm trong quá khứ.");
            }

            if (plantingDate.Value < today)
            {
                return (false, "Ngày gieo trồng nằm trong quá khứ.");
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

            if (createDate > endDate)
            {
                return (false, "Ngày tạo phải trước ngày kết thúc.");
            }
            //logic về khoảng thời gian trong ngày 
            var dates = new[] { createDate.Value, endDate.Value, plantingDate.Value, harvestDate.Value };
            if (dates.Distinct().Count() < dates.Length)
                return (false, "Các ngày không được trùng nhau.");
            if (createDate == endDate)
                return (false, "Ngày tạo và ngày kết thúc không được trùng nhau.");

            if (createDate == plantingDate)
                return (false, "Ngày tạo và ngày gieo trồng không được trùng nhau.");

            if (createDate == harvestDate)
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

                    if (createDate < activityStartDate || createDate > activityEndDate)
                    {
                        return (false, "Ngày tạo phải nằm trong khoảng thời gian của hoạt động.");
                    }

                    if (endDate < activityStartDate || endDate > activityEndDate)
                    {
                        return (false, "Ngày kết thúc phải nằm trong khoảng thời gian của hoạt động.");
                    }

                    if (plantingDate < activityStartDate || plantingDate > activityEndDate)
                    {
                        return (false, "Ngày gieo trồng phải nằm trong khoảng thời gian của hoạt động.");
                    }

                    if (harvestDate < activityStartDate || harvestDate > activityEndDate)
                    {
                        return (false, "Ngày thu hoạch phải nằm trong khoảng thời gian của hoạt động.");
                    }
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
                await _unitOfWork.scheduleRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
                var validate = ValidateScheduleDate(schedule);
                if(validate.Result.Item1 == false)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, validate.Result.Item2);
                }

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

        public Task<ResponseDTO> UpdateSchedulesAsync(long ScheduleId, ScheduleRequest request)
        {
            throw new NotImplementedException();
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



                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);


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

        public Task<ResponseDTO> AssignTask(long scheduleID, long staffId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> UpdateActivities(long ScheduleId, long activityId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> ChangeScheduleStatusById(long ScheduleId, string status)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> ScheduleStaffView(int month)
        {
            throw new NotImplementedException();
        }
    }
         #endregion
}
