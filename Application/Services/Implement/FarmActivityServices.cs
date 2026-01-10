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
        private readonly IScheduleRepository _scheduleRepository;
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

        public async Task<ResponseDTO?> ValidateCreateAsync(FarmActivityRequest request, ActivityType activityType)
        {
            if (request == null)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Dữ liệu yêu cầu không hợp lệ.");

            if (!request.StartDate.HasValue)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Ngày bắt đầu hoạt động là bắt buộc.");

            if (!request.EndDate.HasValue)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Ngày kết thúc hoạt động là bắt buộc.");

            if (request.StartDate.Value > request.EndDate.Value)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

            // 1. ScheduleId bắt buộc và tồn tại
            var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(request.ScheduleId.Value);
            if (schedule == null)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Lịch không tồn tại.");

            // 2. Schedule phải đang ACTIVE
            if (schedule.Status != Status.ACTIVE)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Không thể thêm hoạt động vào lịch trình đã tạm dừng.");

            // 3. Thời gian activity phải nằm trong Schedule
            if (request.StartDate < schedule.StartDate || request.EndDate > schedule.EndDate)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Thời gian hoạt động phải nằm trong khoảng thời gian của lịch.");

            // 4. Nhân viên tồn tại
            var staff = await _unitOfWork.accountRepository.GetByIdAsync(request.StaffId);
            if (staff == null)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Nhân viên được giao không tồn tại.");

            // 5. Nhân viên không được trùng lịch làm việc (quan trọng nhất)
            bool staffConflict = await _farmActivityRepository.HasStaffTimeConflictAsync(
                staffId: request.StaffId,
                startDate: request.StartDate.Value,
                endDate: request.EndDate.Value,
                excludeActivityId: null);

            if (staffConflict)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Nhân viên này đã được giao hoạt động khác trong khoảng thời gian này.");

            // 6. Không trùng hoạt động cùng loại trong cùng lịch trình (tránh duplicate)
            bool hasDuplicateType = await _farmActivityRepository.HasDuplicateActivityTypeInScheduleAsync(
                scheduleId: request.ScheduleId.Value,
                activityType: activityType,
                excludeActivityId: null);

            if (hasDuplicateType)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION,
                    $"Hoạt động này đã tồn tại trong lịch trình này. Không thể tạo thêm.");
            }

            // 7. Kiểm tra phù hợp giai đoạn cây trồng
            if (!IsActivitySuitableForPlantStage(activityType, schedule.currentPlantStage))
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION,
                    $"Hoạt động \"{activityType}\" không phù hợp với giai đoạn cây trồng hiện tại \"{schedule.currentPlantStage}\".");
            }

            return null;
        }

        private bool IsActivitySuitableForPlantStage(ActivityType activityType, PlantStage currentStage)
        {
            return (activityType, currentStage) switch
            {
                // === Preparation (0–7 ngày): Chỉ cho phép chuẩn bị đất và gieo hạt ===
                (ActivityType.SoilPreparation, PlantStage.Preparation) => true,
                (ActivityType.Sowing, PlantStage.Preparation) => true,

                // === Seedling (8–18 ngày): Cây con phát triển rễ, lá non ===
                (ActivityType.Thinning, PlantStage.Seedling) => true,                 // Tỉa cây con
                (ActivityType.Weeding, PlantStage.Seedling) => true,                  // Nhổ cỏ
                (ActivityType.FertilizingDiluted, PlantStage.Seedling) => true,       // Bón phân pha loãng lần đầu
                (ActivityType.PestControl, PlantStage.Seedling) => true,              // Phòng sâu bệnh sinh học
                (ActivityType.FrostProtectionCovering, PlantStage.Seedling) => true,  // Phủ bạt che lạnh (nếu cần)

                // === Vegetative (19–35 ngày): Sinh trưởng mạnh, phát triển lá thân ===
                (ActivityType.Weeding, PlantStage.Vegetative) => true,
                (ActivityType.FertilizingDiluted, PlantStage.Vegetative) => true,     // Bón thúc tiếp
                (ActivityType.FertilizingLeaf, PlantStage.Vegetative) => true,        // Bón phân lá chính ở giai đoạn này
                (ActivityType.PestControl, PlantStage.Vegetative) => true,

                // === Harvest (36–37 ngày): Thu hoạch và kết thúc ===
                (ActivityType.Harvesting, PlantStage.Harvest) => true,
                (ActivityType.CleaningFarmArea, PlantStage.Harvest) => true,          // Dọn dẹp sau thu hoạch
                (ActivityType.PestControl, PlantStage.Harvest) => true,               // Phòng sâu trước thu (nếu dùng thuốc sinh học)

                // Mọi trường hợp khác → KHÔNG phù hợp
                _ => false
            };
        }

        public async Task<ResponseDTO> CreateFarmActivityAsync(FarmActivityRequest farmActivityRequest, ActivityType activityType)
        {
            var validationResponse = await ValidateCreateAsync(farmActivityRequest, activityType);

            // Nếu validate fail → return ngay lỗi
            if (validationResponse != null)
            {
                return validationResponse;
            }

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
            try
            {
                // 1. Tìm FarmActivity
                var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);
                if (farmActivity == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }

                // 2. Xác định trạng thái mới (toggle)
                FarmActivityStatus newStatus = farmActivity.Status == FarmActivityStatus.ACTIVE
                    ? FarmActivityStatus.DEACTIVATED
                    : FarmActivityStatus.ACTIVE;

                // 3. Nếu không thay đổi gì → trả về sớm
                if (farmActivity.Status == newStatus)
                {
                    return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, "Trạng thái không thay đổi.");
                }

                // 4. Validate chỉ khi chuyển sang ACTIVE
                var today = _currentTime.GetCurrentTime().ToDateTime(TimeOnly.MinValue);

                if (newStatus == FarmActivityStatus.ACTIVE)
                {
                    // a. Không cho active lại activity đã hết hạn (EndDate < today)
                    if (farmActivity.EndDate?.ToDateTime(TimeOnly.MinValue) < today)
                    {
                        return new ResponseDTO(Const.ERROR_EXCEPTION, "Không thể kích hoạt lại hoạt động đã kết thúc trong quá khứ.");
                    }

                    // b. Không cho active nếu thời gian chồng chéo với activity ACTIVE khác
                    bool hasTimeOverlap = await _unitOfWork.farmActivityRepository.HasOverlappingActiveActivityAsync(
                        scheduleId: farmActivity.scheduleId.Value,
                        startDate: farmActivity.StartDate.Value,
                        endDate: farmActivity.EndDate.Value,
                        excludeActivityId: farmActivity.FarmActivitiesId);  // exclude chính activity đang toggle

                    if (hasTimeOverlap)
                    {
                        return new ResponseDTO(Const.ERROR_EXCEPTION,
                            $"Không thể kích hoạt hoạt động vì thời gian từ {farmActivity.StartDate:dd/MM/yyyy} đến {farmActivity.EndDate:dd/MM/yyyy} đang chồng chéo với hoạt động khác đang active.");
                    }

                    // c. Staff không được trùng lịch với activity ACTIVE khác
                    bool staffConflict = await _unitOfWork.farmActivityRepository.HasStaffTimeConflictAsync(
                        staffId: farmActivity.AssignedTo,
                        startDate: farmActivity.StartDate.Value,
                        endDate: farmActivity.EndDate.Value,
                        excludeActivityId: farmActivity.FarmActivitiesId);  // exclude chính nó

                    if (staffConflict)
                    {
                        return new ResponseDTO(Const.ERROR_EXCEPTION,
                            "Nhân viên được giao đã có hoạt động khác trùng thời gian trong khoảng này.");
                    }
                }
                // Khi chuyển sang DEACTIVATED → luôn cho phép, không validate

                // 5. Cập nhật trạng thái mới
                farmActivity.Status = newStatus;

                //Cập nhật UpdatedAt, UpdatedBy
                //farmActivity.UpdatedAt = _currentTime.GetCurrentTime().ToDateTime(TimeOnly.MinValue);
                //farmActivity.updatedBy = currentUser?.AccountId ?? 0; 

                await _unitOfWork.farmActivityRepository.UpdateAsync(farmActivity);
                if (await _unitOfWork.SaveChangesAsync() < 0)
                {
                    return new ResponseDTO(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }

                // 6. Trả kết quả
                string action = newStatus == FarmActivityStatus.ACTIVE ? "kích hoạt" : "tạm dừng";
                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, $"Đã {action} hoạt động thành công.");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Đã xảy ra lỗi khi thay đổi trạng thái hoạt động.");
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
                scheduleId = fa.scheduleId,
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
                scheduleId = fa.scheduleId,
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
                scheduleId = fa.scheduleId,
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
                scheduleId = fa.scheduleId,
                StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                StaffEmail = fa.AssignedToNavigation?.Email,
                StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
            };

            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, view);
        }


        public async Task<ResponseDTO> UpdateFarmActivityAsync(long farmActivityId, FarmActivityRequest farmActivityrequest, ActivityType activityType, FarmActivityStatus farmActivityStatus)
        {
            var validationResponse = await ValidateCreateAsync(farmActivityrequest, activityType);

            // Nếu validate fail → return ngay lỗi
            if (validationResponse != null)
            {
                return validationResponse;
            }

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
