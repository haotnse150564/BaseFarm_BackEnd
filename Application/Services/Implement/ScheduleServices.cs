using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.ViewModel.Request;
using Microsoft.Extensions.Configuration;
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


        public async Task<ResponseDTO> CreateScheduleAsync(ScheduleRequest request)
        {
            try
            {
                var crop = await _unitOfWork.cropRepository.GetByIdAsync(request.CropId);
                var ReqCrop = await _unitOfWork.cropRequirementRepository.GetByIdAsync(crop.CropRequirement.RequirementId);
                var result = new Schedule
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    //EndDate = DateOnly.Parse("09/09/2025"),
                    AssignedTo = request.AssignedTo,
                    //FarmActivityId = request.FarmActivityId,
                    PlantingDate = request.PlantingDate,
                    HarvestDate = request.PlantingDate.Value.AddDays((int)ReqCrop.EstimatedDate),
                    FarmDetailsId = request.FarmDetailsId,
                    CropId = request.CropId,
                    UpdatedAt = _currentTime.GetCurrentTime(),
                    CreatedAt = _currentTime.GetCurrentTime(),
                    Status = Status.ACTIVE,
                };
                #region checkValidate and add FarmActivity
                if (result.StartDate < DateOnly.FromDateTime(DateTime.Today) || result.EndDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "Start Date and End Date at least is today");
                }
                else if (result.StartDate >= result.EndDate)
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "The start date cannot be set after the end date.");
                }
                else if (result.StartDate > result.EndDate)
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "The start date cannot be set before the end date.");
                }
                else
                {
                    await _unitOfWork.scheduleRepository.AddAsync(result);
                    //check farmActivity is ACTIVE and date range match with Schedule date range
                    foreach (var farmActId in request.FarmActivityId)
                    {
                        var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActId);
                        if (farmActivity.Status != Domain.Enum.FarmActivityStatus.ACTIVE)
                            return new ResponseDTO(Const.FAIL_READ_CODE, "All Farm Activity must in ACTIVE status.");
                    }
                    foreach (var farmActivityId in request.FarmActivityId)
                    {
                        var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);
                        if (farmActivity == null)
                        {
                            return new ResponseDTO(Const.FAIL_READ_CODE, "No Farm Activity found with the given ID.");
                        }
                        if (farmActivity.Status != FarmActivityStatus.ACTIVE)
                        {
                            return new ResponseDTO(Const.FAIL_READ_CODE, "Some Farm Activity is in process or deactived.");
                        }
                        else if (farmActivity.StartDate < result.StartDate && farmActivity.EndDate > result.EndDate)
                        {
                            return new ResponseDTO(Const.FAIL_READ_CODE, "Farm Activity date range does not match with Schedule date range.");
                        }
                        else if (farmActivity.ScheduleId != null && farmActivity.ScheduleId != 0)
                        {
                            return new ResponseDTO(Const.FAIL_READ_CODE, "Farm Activity is already assigned to another Schedule.");
                        }
                        else
                        {
                            farmActivity.ScheduleId = result.ScheduleId;
                            farmActivity.Status = FarmActivityStatus.IN_PROGRESS; // Cập nhật trạng thái FarmActivity
                            await _unitOfWork.farmActivityRepository.UpdateAsync(farmActivity);
                        }
                    }
                    #endregion
                    await _unitOfWork.SaveChangesAsync();

                    var resultView = _mapper.Map<ViewSchedule>(result);
                    return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, resultView);
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<ResponseDTO> ChangeScheduleStatusById(long ScheduleId, string status)
        {
            try
            {
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(ScheduleId);

                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Schedule found.");
                }

                // Map dữ liệu sang DTO
                schedule.Status = (Status)Enum.Parse(typeof(Status), status); // Chuyển chuỗi sang Enum
                await _unitOfWork.scheduleRepository.UpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<ViewSchedule>(schedule);
                result.FullNameStaff = (await _account.GetAccountProfileByAccountIdAsync(schedule.AssignedTo)).AccountProfile?.Fullname;
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetAllScheduleAsync(int pageIndex, int pageSize)
        {
            try
            {
                var list = await _unitOfWork.scheduleRepository.GetAllAsync();

                if (list == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Schedule found.");
                }

                // Map dữ liệu sang DTO
                var result = _mapper.Map<List<ViewSchedule>>(list);
                var totalItem = result.Count();
                foreach (var item in result)
                {

                    item.FullNameStaff = (await _account.GetAccountProfileByAccountIdAsync(item.AssignedTo)).AccountProfile?.Fullname;

                }
                //await _unitOfWork.cropRepository.GetAllAsync();
                //await _unitOfWork.farmActivityRepository.GetAllAsync();
                //await _unitOfWork.farmRepository.GetAllAsync();
                //await _unitOfWork.accountRepository.GetAllAsync();

                // Tạo đối tượng phân trang
                var pagination = new Pagination<ViewSchedule>
                {
                    TotalItemCount = totalItem,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagination);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<ResponseDTO> UpdateScheduleById(long ScheduleId, ScheduleRequest request)
        {
            try
            {
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(ScheduleId);

                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Schedule found.");
                }

                var update = _mapper.Map(request, schedule);
                update.UpdatedAt = _currentTime.GetCurrentTime();
                // Map dữ liệu sang DTO
                _unitOfWork.scheduleRepository.Update(update);

                var oldListFarmActivity = await _unitOfWork.farmActivityRepository.GetListFarmActivityByScheduleId(ScheduleId);
                var newListFarmActivity = await _unitOfWork.farmActivityRepository.GetListFarmActivityUpdate(request.FarmActivityId);
                
                var notInListFarmActivity = oldListFarmActivity.Except(newListFarmActivity).ToList();
                var InListFarmActivity = newListFarmActivity.Except(oldListFarmActivity).ToList();

                // Xử lý các FarmActivity không còn trong danh sách
                foreach (var item in notInListFarmActivity)
                {
                    item.ScheduleId = null; // Xóa liên kết với Schedule
                    item.Status = FarmActivityStatus.ACTIVE; // Đặt lại trạng thái FarmActivity
                    await _unitOfWork.farmActivityRepository.UpdateAsync(item);
                }
                // Xử lý các FarmActivity mới được thêm vào
                foreach (var item in InListFarmActivity)
                {
                    if (item.Status != FarmActivityStatus.ACTIVE)
                    {
                        return new ResponseDTO(Const.FAIL_READ_CODE, "Some Farm Activity is in process or deactived.");
                    }
                    else if (item.StartDate < update.StartDate && item.EndDate > update.EndDate)
                    {
                        return new ResponseDTO(Const.FAIL_READ_CODE, "Farm Activity date range does not match with Schedule date range.");
                    }
                    else if (item.ScheduleId != null && item.ScheduleId != 0)
                    {
                        return new ResponseDTO(Const.FAIL_READ_CODE, "Farm Activity is already assigned to another Schedule.");
                    }
                    else
                    {
                        item.ScheduleId = ScheduleId; // Gán lại ScheduleId
                        item.Status = FarmActivityStatus.IN_PROGRESS; // Cập nhật trạng thái FarmActivity
                        await _unitOfWork.farmActivityRepository.UpdateAsync(item);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                //xu ly inventory neu farmactivitytype là harvest và status là completed
                var farmActivity = await _unitOfWork.farmActivityRepository.GetHarvestFarmActivityId(ScheduleId);
                if (farmActivity != null && farmActivity.ActivityType == Domain.Enum.ActivityType.Harvesting && farmActivity.Status == FarmActivityStatus.COMPLETED)
                {
                    await _inventory.CalculateAndCreateInventoryAsync(schedule.Quantity, request.Location, schedule.CropId, ScheduleId);
                }

                var result = _mapper.Map<ViewSchedule>(update);
                result.FullNameStaff = (await _account.GetAccountProfileByAccountIdAsync(result.AssignedTo)).AccountProfile?.Fullname;
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<ResponseDTO> GetScheduleByIdAsync(long ScheduleId)
        {
            try
            {
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(ScheduleId);

                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Schedule found.");
                }

                // Map dữ liệu sang DTO
                var result = _mapper.Map<ViewSchedule>(schedule);
                result.FullNameStaff = (await _account.GetAccountProfileByAccountIdAsync(schedule.AssignedTo)).AccountProfile?.Fullname;
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> AssignStaff(long scheduleID, long staffId)
        {
            try
            {
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(scheduleID);

                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Schedule found.");
                }

                // Map dữ liệu sang DTO
                schedule.AssignedTo = staffId;
                await _unitOfWork.scheduleRepository.UpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<ViewSchedule>(schedule);
                result.FullNameStaff = (await _account.GetAccountProfileByAccountIdAsync(schedule.AssignedTo)).AccountProfile?.Fullname;
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetScheduleByCurrentStaffAsync(int month)
        {
            try
            {
                var user = await _jwtUtils.GetCurrentUserAsync();
                var schedule = await _unitOfWork.scheduleRepository.GetByStaffIdAsync(user.AccountId, month);

                if (schedule.Count == 0)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Schedule found.");
                }

                // Map dữ liệu sang DTO
                var result = _mapper.Map<List<ViewSchedule>>(schedule);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
