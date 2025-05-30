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
        public ScheduleServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper
                , IAccountRepository account, ICropRepository cropRepositorym, IFarmActivityRepository farmActivityRepository
                , IFarmRepository farmRepository, JWTUtils jwtUtils)
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
        }


        public async Task<ResponseDTO> CreateScheduleAsync(ScheduleRequest request)
        {
            try
            {
                var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(request.FarmActivityId);
                var result = new Schedule
                {
                    StartDate = request.StartDate,
                    //EndDate = request.EndDate,
                    EndDate = DateOnly.Parse("09/09/2025"),
                    AssignedTo = request.AssignedTo,
                    //FarmActivityId = request.FarmActivityId,
                    PlantingDate = request.PlantingDate,
                    FarmActivities = farmActivity == null ? new List<FarmActivity>() : new List<FarmActivity> { farmActivity },
                    FarmDetailsId = request.FarmDetailsId,
                    CropId = request.CropId,
                    UpdatedAt = _currentTime.GetCurrentTime(),
                    CreatedAt = _currentTime.GetCurrentTime(),
                    Status = Status.ACTIVE,
                };
                var listSchedule = await _unitOfWork.scheduleRepository.GetAllAsync();
                var getduplicateCrop = listSchedule.FirstOrDefault(x => x.CropId == result.CropId && x.Status == Status.ACTIVE);
                if (result.StartDate < DateOnly.FromDateTime(DateTime.Today) || result.EndDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "Start Date and End Date at least is today");
                }
                else if (result.StartDate >= result.EndDate)
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "The start date cannot be set after the end date.");
                }
                else if (getduplicateCrop.EndDate >= result.StartDate)
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "Crop is in the process of cultivation");
                }
                else if (result.StartDate > result.EndDate)
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "The start date cannot be set before the end date.");
                }
                else
                {


                    // Map dữ liệu sang DTO
                    await _unitOfWork.scheduleRepository.AddAsync(result);
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
                await _unitOfWork.SaveChangesAsync();

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

        public async Task<ResponseDTO> GetScheduleByCurrentStaffAsync()
        {
            try
            {
                var user = await _jwtUtils.GetCurrentUserAsync();
                var schedule = await _unitOfWork.scheduleRepository.GetByStaffIdAsync(user.AccountId);

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
