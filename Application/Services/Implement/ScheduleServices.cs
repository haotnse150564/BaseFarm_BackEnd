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
