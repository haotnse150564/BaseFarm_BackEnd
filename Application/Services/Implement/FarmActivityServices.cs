using Application;
using Application.Commons;
using Application.Interfaces;
using Application.Services;
using Application.Utils;
using AutoMapper;
using Azure.Core;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.ViewModel.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI.Services
{
    public class FarmActivityServices : IFarmActivityServices

    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly IFarmActivityRepository _farmActivityRepository;
        private readonly IInventoryService _inventory;
        private readonly JWTUtils _jwtUtils;
        private readonly IScheduleLogRepository _scheduleLogRepo;
        public FarmActivityServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper
            , IFarmActivityRepository farmActivityRepository, IInventoryService inventory, JWTUtils jWTUtils, IScheduleLogRepository scheduleLogRepo)

        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _farmActivityRepository = farmActivityRepository;
            _inventory = inventory;
            _jwtUtils = jWTUtils;
            _scheduleLogRepo = scheduleLogRepo;
        }

        public async Task<ResponseDTO> CreateFarmActivityAsync(FarmActivityRequest farmActivityRequest, ActivityType activityType)
        {
            var utcDate = DateTime.UtcNow.ToUniversalTime();

            var user = await _jwtUtils.GetCurrentUserAsync();
            var farmActivity = _mapper.Map<FarmActivity>(farmActivityRequest);
            farmActivity.Status = Domain.Enum.FarmActivityStatus.ACTIVE;
            farmActivity.ActivityType = activityType;
            farmActivity.CreatedAt = utcDate;
            farmActivity.createdBy = user.AccountId;
            farmActivity.UpdatedAt = DateTime.UtcNow;
            farmActivity.updatedBy = user.AccountId;
            

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
                farmActivity.Status = (farmActivity.Status == Domain.Enum.FarmActivityStatus.ACTIVE) ? Domain.Enum.FarmActivityStatus.DEACTIVATED : Domain.Enum.FarmActivityStatus.ACTIVE;
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

        public async Task<ResponseDTO> GetFarmActivitiesActiveAsync(int pageIndex,int pageSize)
        {
            var list = await _unitOfWork.farmActivityRepository.GetAllActive();

            if (list.IsNullOrEmpty())
            {
                return new ResponseDTO(Const.FAIL_READ_CODE,Const.FAIL_READ_MSG,"Not Found");
            }

            // ===== Map tay, không dùng AutoMapper =====
            var views = list.Select(fa => new FarmActivityView
            {
                FarmActivitiesId = fa.FarmActivitiesId,
                ActivityType = fa.ActivityType?.ToString(),
                StartDate = fa.StartDate?.ToString("dd/MM/yyyy"),
                EndDate = fa.EndDate?.ToString("dd/MM/yyyy"),
                Status = fa.Status?.ToString(),

                CropName = fa.Schedule?.Crop?.CropName,
                StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                StaffEmail = fa.AssignedToNavigation?.Email,
                StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
            }).ToList();

            var pagination = new Pagination<FarmActivityView>
            {
                TotalItemCount = views.Count,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = views.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };

            return new ResponseDTO(Const.SUCCESS_READ_CODE,Const.SUCCESS_READ_MSG,pagination);
        }


        public async Task<ResponseDTO> GetFarmActivitiesAsync(int pageIndex, int pageSize, ActivityType? type, FarmActivityStatus? status, int? month)
        {
            var result = await _unitOfWork.farmActivityRepository.GetAllFiler(type, status, month);

            if (result.IsNullOrEmpty())
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Not Found");

            result.Sort((y, x) => x.FarmActivitiesId.CompareTo(y.FarmActivitiesId));

            var views = result.Select(fa => new FarmActivityView
            {
                FarmActivitiesId = fa.FarmActivitiesId,
                ActivityType = fa.ActivityType.ToString(),
                StartDate = fa.StartDate?.ToString("dd/MM/yyyy"),
                EndDate = fa.EndDate?.ToString("dd/MM/yyyy"),
                Status = fa.Status.ToString(),

                CropName = fa.Schedule?.Crop?.CropName,
                StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                StaffEmail = fa.AssignedToNavigation?.Email,
                StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
            }).ToList();

            var pagination = new Pagination<FarmActivityView>
            {
                TotalItemCount = views.Count,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = views.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };

            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagination);
        }
        public async Task<ResponseDTO> GetFarmActivitiesByStaffAsync(int pageIndex,int pageSize,ActivityType? type,FarmActivityStatus? status,int? month)
        {
            var result = await _unitOfWork.farmActivityRepository.GetAllFiler(type, status, month);

            if (result.IsNullOrEmpty())
            {
                return new ResponseDTO(Const.FAIL_READ_CODE,Const.FAIL_READ_MSG,"Not Found");
            }

            result.Sort((y, x) => x.FarmActivitiesId.CompareTo(y.FarmActivitiesId));

            var views = result.Select(fa => new FarmActivityView
            {
                FarmActivitiesId = fa.FarmActivitiesId,
                ActivityType = fa.ActivityType?.ToString(),
                StartDate = fa.StartDate?.ToString("dd/MM/yyyy"),
                EndDate = fa.EndDate?.ToString("dd/MM/yyyy"),
                Status = fa.Status?.ToString(),

                CropName = fa.Schedule?.Crop?.CropName,
                StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                StaffEmail = fa.AssignedToNavigation?.Email,
                StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
            }).ToList();

            var pagination = new Pagination<FarmActivityView>
            {
                TotalItemCount = views.Count,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = views.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };

            return new ResponseDTO(Const.SUCCESS_READ_CODE,Const.SUCCESS_READ_MSG,pagination);
        }


        public async Task<ResponseDTO> GetFarmActivityByIdAsync(long farmActivityId)
        {
            var farmActivity = await _unitOfWork.farmActivityRepository.GetAllFiler(null, null, null);

            var fa = farmActivity.FirstOrDefault(x => x.FarmActivitiesId == farmActivityId);

            if (fa == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }

            var view = new FarmActivityView
            {
                FarmActivitiesId = fa.FarmActivitiesId,
                ActivityType = fa.ActivityType?.ToString(),
                StartDate = fa.StartDate?.ToString("dd/MM/yyyy"),
                EndDate = fa.EndDate?.ToString("dd/MM/yyyy"),
                Status = fa.Status?.ToString(),

                CropName = fa.Schedule?.Crop?.CropName,
                StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                StaffEmail = fa.AssignedToNavigation?.Email,
                StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
            };

            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, view);
        }


        public async Task<ResponseDTO> UpdateFarmActivityAsync(long farmActivityId, FarmActivityRequest farmActivityrequest, ActivityType? activityType, FarmActivityStatus farmActivityStatus)
        {
            var user = await _jwtUtils.GetCurrentUserAsync();
            var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);

            if (farmActivity == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            else
            {
                farmActivity.ActivityType = activityType;
                farmActivity.Status = farmActivityStatus; 
                farmActivity.StartDate = farmActivityrequest.StartDate;
                farmActivity.EndDate = farmActivityrequest.EndDate;
                farmActivity.UpdatedAt = DateTime.UtcNow;
                farmActivity.updatedBy = user.AccountId;
                var checkUpdate = await _unitOfWork.farmActivityRepository.UpdateAsync(farmActivity);
                if (checkUpdate < 0)
                {
                    return new ResponseDTO(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
                else
                {
                    if (farmActivity.ActivityType == ActivityType.Harvesting && farmActivity.Status == FarmActivityStatus.COMPLETED)
                    {
                        var product = await _unitOfWork.farmActivityRepository.GetProductWillHarves(farmActivity.FarmActivitiesId);
                        if (product == null)
                        {
                            return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                        }
                        //foreach (var item in product)
                        //{
                        //    //CỘNG SL KHI THU HOẠCH
                        //    var schedule = await _unitOfWork.scheduleRepository.GetByCropId(item.ProductId);
                        //    item.StockQuantity += schedule.Quantity;
                        //}
                        if (await _unitOfWork.SaveChangesAsync() < 0)
                        {
                            return new ResponseDTO(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                        }
                        else
                        {
                            return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                        }
                    }
                    var result = _mapper.Map<FarmActivityView>(farmActivity);
                    return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);
                }
            }
        }
        public async Task<ResponseDTO> CompleteFarmActivity(long id, string? location)
        {
            var farmActivity = await _farmActivityRepository.GetByIdAsync(id);
            if (farmActivity == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Not Found Farm Activity");
            }//else if (farmActivity.ScheduleId == null)
             // {
             // return new ResponseDTO(Const.FAIL_READ_CODE, "Farm Activity Don't Have Any Schedule");
             // }    
             //else if (farmActivity.Status != FarmActivityStatus.IN_PROGRESS)
             //{
             //    return new ResponseDTO(Const.FAIL_READ_CODE, "Farm Activity Already Completed or do not used by any Schedule");
             //}

            var user = await _jwtUtils.GetCurrentUserAsync();
            if (user == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Người dùng không hợp lệ");
            }

            farmActivity.Status = Domain.Enum.FarmActivityStatus.COMPLETED; 

            await _unitOfWork.farmActivityRepository.UpdateAsync(farmActivity);

            var Schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(farmActivity.scheduleId ?? 0);
            if (Schedule == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Schedule not found!");
            }
            var scheduleLog = new ScheduleLog
            {
                ScheduleId = (long)farmActivity.scheduleId,
                Notes = $"[Ghi chú tự động] Hoạt động {farmActivity.ActivityType} đã được {farmActivity.AssignedToNavigation.AccountProfile.Fullname} hoàn thành",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.AccountId,
            };
            await _scheduleLogRepo.AddAsync(scheduleLog);
            await _unitOfWork.SaveChangesAsync();

            if (farmActivity.ActivityType == ActivityType.Harvesting)
            {
                var product = await _unitOfWork.farmActivityRepository.GetProductWillHarves(farmActivity.FarmActivitiesId);
                if (product == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                foreach(var item in product)
                {
                    //CỘNG SL KHI THU HOẠCH
                    var schedule = await _unitOfWork.scheduleRepository.GetByCropId(item.ProductId);
                    item.StockQuantity += schedule.Quantity;
                }
                if (await _unitOfWork.SaveChangesAsync() < 0)
                {
                    return new ResponseDTO(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
                else
                {
                    return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                }
            }
            else
            {
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
        public virtual bool CheckDate(DateOnly? startDate, DateOnly? endDate)
        {
            if (startDate < _currentTime.GetCurrentTime() || endDate < _currentTime.GetCurrentTime())
            {
                return false;
            }
            else if (startDate > endDate)
            {
                return false;
            }
            else if((startDate.Value.DayNumber - endDate.Value.DayNumber) >7)
            {
                return false;
            }
            return true;
        }
    }
}
