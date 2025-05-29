using Application.Interfaces;
using Application;
using Application.Services;
using AutoMapper;
using Infrastructure.ViewModel.Response;
using Microsoft.Extensions.Configuration;
using Infrastructure.Repositories;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.ViewModel.Request;
using Domain.Model;
using Domain.Enum;
using Application.Utils;

namespace WebAPI.Services
{
    public class FarmActivityServices : IFarmActivityServices

    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly IFarmActivityRepository _farmActivityRepository;
        public FarmActivityServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, IFarmActivityRepository farmActivityRepository)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _farmActivityRepository = farmActivityRepository;
        }

        public async Task<ResponseDTO> CreateFarmActivityAsync(FarmActivityRequest farmActivityRequest, ActivityType activityType)
        {
            var farmActivity = _mapper.Map<FarmActivity>(farmActivityRequest);
            farmActivity.Status = Domain.Enum.Status.ACTIVE;
            farmActivity.ActivityType = activityType;
            if (!CheckDate(farmActivity.StartDate, farmActivity.EndDate))
            {
                return new ResponseDTO(Const.FAIL_UPDATE_CODE, "Start date or end date is wrong!");
            }
            await _unitOfWork.farmActivityRepository.AddAsync(farmActivity);

            if (await _unitOfWork.SaveChangesAsync() < 0)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Add Fail!");
            }
            else
            {
                var result = _mapper.Map<FarmActivityView>(farmActivity);
                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
            }
        }

        public async Task<ResponseDTO> ChangeFarmActivityStatusAsync(long farmActivityId)
        {
            var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);

            if (farmActivity == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            else
            {
                farmActivity.Status = (farmActivity.Status == Domain.Enum.Status.ACTIVE) ? Domain.Enum.Status.DEACTIVATED : Domain.Enum.Status.ACTIVE;
                await _unitOfWork.farmActivityRepository.UpdateAsync(farmActivity);
                if (await _unitOfWork.SaveChangesAsync() < 0)
                {
                    return new ResponseDTO(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
                else
                {
                    return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                }
            }
        }

        public async Task<ResponseDTO> GetFarmActivitiesActiveAsync()
        {
            var list = await _unitOfWork.farmActivityRepository.GetAllAsync();
            if (list.IsNullOrEmpty())
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            else
            {
                var result = list.Where(x => x.Status == Domain.Enum.Status.ACTIVE).ToList();
                if (list.IsNullOrEmpty())
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                else
                {
                    var resultView = _mapper.Map<List<FarmActivityView>>(result);
                    return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, resultView);
                }
            }
        }

        public async Task<List<FarmActivityView>> GetFarmActivitiesAsync()
        {
            var result = await _unitOfWork.farmActivityRepository.GetAllAsync();
            if (result.IsNullOrEmpty())
            {
                throw new Exception();
            }
            else
            {
                // Map dữ liệu sang DTO
                var resultView = _mapper.Map<List<FarmActivityView>>(result);
                return resultView;
            }

        }

        public async Task<ResponseDTO> GetFarmActivityByIdAsync(long farmActivityId)
        {
            var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);
            if (farmActivity == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            else
            {
                var result = _mapper.Map<FarmActivityView>(farmActivity);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
        }

        public async Task<ResponseDTO> UpdateFarmActivityAsync(long farmActivityId, FarmActivityRequest farmActivityrequest, ActivityType? activityType)
        {
            var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);

            if (farmActivity == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            else
            {
                farmActivity.StartDate = farmActivityrequest.StartDate;
                farmActivity.EndDate = farmActivityrequest.EndDate;
                farmActivity.ActivityType = activityType;
                farmActivity.ScheduleId = farmActivityrequest.ScheduleId;
                if(!CheckDate(farmActivity.StartDate, farmActivity.EndDate))
                {
                    return new ResponseDTO(Const.FAIL_UPDATE_CODE, "Start date or end date is wrong!");
                }   
                await _unitOfWork.farmActivityRepository.UpdateAsync(farmActivity);
                if (await _unitOfWork.SaveChangesAsync() < 0)
                {
                    return new ResponseDTO(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
                else
                {
                    var result = _mapper.Map<FarmActivityView>(farmActivity);
                    return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);
                }
            }
        }
        public bool CheckDate(DateOnly? startDate, DateOnly? endDate)
        {
            if (startDate < _currentTime.GetCurrentTime() || endDate < _currentTime.GetCurrentTime())
            {
                return false;
            }
            else if (startDate > endDate)
            {
                return false;
            }
            return true;
        }
    }
}
