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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;
using static Infrastructure.ViewModel.Response.StaffActivityResponse;

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

            // 3. Thời gian activity phải nằm trong khoảng của lịch
            // (cho phép lố ra ngoài ngày kết thúc dự kiến)
            if (request.StartDate.Value < schedule.StartDate)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION,
                    "Ngày bắt đầu hoạt động phải từ ngày bắt đầu của lịch trình trở đi.");
            }

            // 4. Nhân viên tồn tại
            //var staff = await _unitOfWork.accountRepository.GetByIdAsync(request.StaffId);
            //if (staff == null)
            //    return new ResponseDTO(Const.ERROR_EXCEPTION, "Nhân viên được giao không tồn tại.");

            //// 5. Nhân viên không được trùng lịch làm việc (quan trọng nhất)
            //bool staffConflict = await _farmActivityRepository.HasStaffTimeConflictAsync(
            //    staffId: request.StaffId,
            //    startDate: request.StartDate.Value,
            //    endDate: request.EndDate.Value,
            //    excludeActivityId: null);

            //if (staffConflict)
            //    return new ResponseDTO(Const.ERROR_EXCEPTION, "Nhân viên này đã được giao hoạt động khác trong khoảng thời gian này.");

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

            // 8. Check chồng chéo hoạt động trong cùng schedule
            bool hasOverlap = await _farmActivityRepository.HasOverlappingActivityInScheduleAsync(
                scheduleId: request.ScheduleId.Value,
                startDate: request.StartDate.Value,
                endDate: request.EndDate.Value,
                excludeActivityId: null);

            if (hasOverlap)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION,
                    "Khoảng thời gian này đã có hoạt động khác trong lịch trình. Kiểm tra lại để tránh chồng hoạt động.");
            }

            return null;
        }

        public async Task<ResponseDTO?> ValidateUpdateAsync(long farmActivityId, FarmActivityRequest request, ActivityType newActivityType, FarmActivityStatus newStatus, FarmActivity existingActivity)
        {
            if (request == null)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Dữ liệu yêu cầu không hợp lệ.");

            // 1. validate ngày tháng, required fields
            if (!request.StartDate.HasValue || !request.EndDate.HasValue)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Ngày bắt đầu/kết thúc là bắt buộc.");

            if (request.StartDate.Value > request.EndDate.Value)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Ngày bắt đầu phải <= ngày kết thúc.");

            // 2. Không cho thay đổi ScheduleId
            if (request.ScheduleId.HasValue && request.ScheduleId != existingActivity.scheduleId)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Không thể thay đổi lịch trình của hoạt động đã tạo.");

            // 3. Không cho thay đổi ActivityType
            //if (newActivityType != existingActivity.ActivityType)
            //{
            //    return new ResponseDTO(Const.ERROR_EXCEPTION, "Không thể thay đổi loại hoạt động sau khi tạo.");
            //}

            // 4. Lấy schedule hiện tại
            var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync((long)existingActivity.scheduleId);
            if (schedule == null)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Lịch trình không còn tồn tại.");

            if (schedule.Status != Status.ACTIVE)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Không thể cập nhật hoạt động trong lịch đã tạm dừng.");

            // 5. StartDate không được nhỏ hơn StartDate của schedule
            if (request.StartDate.Value < schedule.StartDate)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Ngày bắt đầu phải từ ngày bắt đầu lịch trình trở đi.");
            }

            // 6. Giới hạn EndDate không quá xa
            //var maxOverdueDays = 10;
            //if (request.EndDate.Value > schedule.EndDate.AddDays(maxOverdueDays))
            //{
            //    return new ResponseDTO(..., $"Ngày kết thúc không được vượt quá {maxOverdueDays} ngày sau lịch.");
            //}

            // 7. Check duplicate type: exclude chính activity hiện tại
            bool hasDuplicate = await _farmActivityRepository.HasDuplicateActivityTypeInScheduleAsync(
                scheduleId: (long)existingActivity.scheduleId,
                activityType: newActivityType,
                excludeActivityId: farmActivityId);

            if (hasDuplicate)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Loại hoạt động này đã tồn tại trong lịch trình.");

            // 8. Phù hợp giai đoạn cây – dùng giai đoạn HIỆN TẠI của schedule
            if (!IsActivitySuitableForPlantStage(newActivityType, schedule.currentPlantStage))
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, $"Hoạt động không phù hợp với giai đoạn cây hiện tại: {schedule.currentPlantStage}");
            }

            // 9. Validate trạng thái chuyển đổi (rất quan trọng cho update)
            if (existingActivity.Status == FarmActivityStatus.COMPLETED)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Không thể cập nhật hoạt động đã hoàn thành.");
            }

            //if (newStatus == FarmActivityStatus.COMPLETED &&
            //    newActivityType != ActivityType.Harvesting) // ví dụ: chỉ harvesting mới được hoàn thành để cộng kho
            //{
            //}

            // 10. Check chồng chéo khi update
            bool hasOverlap = await _farmActivityRepository.HasOverlappingActivityInScheduleAsync(
                scheduleId: (long)existingActivity.scheduleId,
                startDate: request.StartDate.Value,
                endDate: request.EndDate.Value,
                excludeActivityId: farmActivityId);  // exclude chính nó

            if (hasOverlap)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION,
                    "Khoảng thời gian mới có chồng chéo với hoạt động khác trong lịch trình.");
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
                (ActivityType.FrostProtectionCovering, PlantStage.Vegetative) => true,  // Phủ bạt che lạnh (nếu cần)

                // === Harvest (36–37 ngày): Thu hoạch và kết thúc ===
                (ActivityType.Harvesting, PlantStage.Harvest) => true,
                (ActivityType.CleaningFarmArea, PlantStage.Harvest) => true,          // Dọn dẹp sau thu hoạch

                // Mọi trường hợp khác → KHÔNG phù hợp
                _ => false
            };
        }

        private async Task<Response_DTO?> ValidateAddStaffToFarmActivityAsync(long farmActivityId, long staffId)
        {

            // 1. Kiểm tra hoạt động tồn tại và trạng thái hợp lệ
            var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);
            if (farmActivity == null)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Không tìm thấy hoạt động.");
            }

            if (farmActivity.Status == FarmActivityStatus.COMPLETED || farmActivity.Status == FarmActivityStatus.DEACTIVATED)
            {
                return new Response_DTO(Const.ERROR_EXCEPTION, "Không thể gán nhân viên cho hoạt động đã hoàn thành hoặc đã hủy.");
            }

            // 2. Kiểm tra nhân viên tồn tại và vai trò phù hợp
            var staff = await _unitOfWork.accountRepository.GetByIdAsync(staffId);
            if (staff == null)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Không tìm thấy nhân viên.");
            }

            if (staff.Role != Roles.Staff && staff.Role != Roles.Staff)
            {
                return new Response_DTO(Const.ERROR_EXCEPTION, "Chỉ có thể gán nhân viên có vai trò Staff");
            }

            // 3. Không cho thêm trùng nhân viên vào cùng hoạt động
            bool alreadyAssigned = await _unitOfWork.staff_FarmActivityRepository.GetQueryable()
                .AnyAsync(s => s.FarmActivityId == farmActivityId && s.AccountId == staffId);

            if (alreadyAssigned)
            {
                return new Response_DTO(Const.ERROR_EXCEPTION, "Nhân viên này đã được gán cho hoạt động này rồi.");
            }

            // 4. Kiểm tra conflict thời gian (rất quan trọng)
            bool hasConflict = await _unitOfWork.staff_FarmActivityRepository
                .HasStaffTimeConflictAsync(
                    staffId: staffId,
                    startDate: (DateOnly)farmActivity.StartDate,
                    endDate: (DateOnly)farmActivity.EndDate,
                    excludeStaffFarmActivityId: null); // null vì là thêm mới

            if (hasConflict)
            {
                return new Response_DTO(Const.ERROR_EXCEPTION,
                    "Nhân viên này đã có lịch làm việc trùng thời gian với hoạt động này.");
            }

            return null; // hợp lệ
        }

        private async Task<Response_DTO?> ValidateUpdateStaffToFarmActivityAsync(long staffFarmActivityId, long newStaffId)
        {

            // 1. Kiểm tra bản ghi Staff_FarmActivity tồn tại
            var staffFarmActivity = await _unitOfWork.staff_FarmActivityRepository
                .GetByIdAsync(staffFarmActivityId);

            if (staffFarmActivity == null)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Không tìm thấy bản ghi gán nhân viên.");
            }

            // 2. Lấy thông tin hoạt động để check trạng thái
            var farmActivity = await _unitOfWork.farmActivityRepository
                .GetByIdAsync(staffFarmActivity.FarmActivityId);

            if (farmActivity == null)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Hoạt động liên quan không tồn tại.");
            }

            if (farmActivity.Status == FarmActivityStatus.COMPLETED ||
                farmActivity.Status == FarmActivityStatus.DEACTIVATED)
            {
                return new Response_DTO(Const.ERROR_EXCEPTION,
                    "Không thể thay đổi nhân viên cho hoạt động đã hoàn thành hoặc đã hủy.");
            }

            // 3. Kiểm tra nhân viên mới tồn tại và vai trò phù hợp
            var newStaff = await _unitOfWork.accountRepository.GetByIdAsync(newStaffId);
            if (newStaff == null)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Không tìm thấy nhân viên.");
            }

            if (newStaff.Role != Roles.Staff && newStaff.Role != Roles.Staff)
            {
                return new Response_DTO(Const.ERROR_EXCEPTION,
                    "Nhân viên mới phải có vai trò Staff.");
            }

            // 4. Không cho thay đổi thành chính nhân viên cũ 
            if (staffFarmActivity.AccountId == newStaffId)
            {
                return new Response_DTO(Const.ERROR_EXCEPTION,
                    "Nhân viên mới trùng với nhân viên hiện tại, không cần cập nhật.");
            }

            // 5. Kiểm tra nhân viên mới chưa được gán cho hoạt động này
            bool alreadyAssigned = await _unitOfWork.staff_FarmActivityRepository
                .GetQueryable()
                .AnyAsync(s => s.FarmActivityId == staffFarmActivity.FarmActivityId
                            && s.AccountId == newStaffId
                            && s.Staff_FarmActivityId != staffFarmActivityId);

            if (alreadyAssigned)
            {
                return new Response_DTO(Const.ERROR_EXCEPTION,
                    "Nhân viên mới đã được gán cho hoạt động này từ bản ghi khác.");
            }

            // 6. Kiểm tra conflict thời gian cho nhân viên MỚI
            bool hasConflict = await _unitOfWork.staff_FarmActivityRepository
                .HasStaffTimeConflictAsync(
                    staffId: newStaffId,
                    startDate: (DateOnly)farmActivity.StartDate,
                    endDate: (DateOnly)farmActivity.EndDate,
                    excludeStaffFarmActivityId: staffFarmActivityId); // exclude bản ghi đang sửa

            if (hasConflict)
            {
                return new Response_DTO(Const.ERROR_EXCEPTION,
                    "Nhân viên mới có lịch làm việc trùng với khoảng thời gian của hoạt động.");
            }

            // 8. (Tùy chọn) Cảnh báo hoặc chặn nếu hoạt động đã bắt đầu
            if (farmActivity.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                // Có thể chỉ cảnh báo, hoặc block tùy nghiệp vụ
                // return new Response_DTO(Const.WARNING, "Hoạt động đã bắt đầu, việc thay đổi nhân viên chỉ nên thực hiện khi cần thiết.");
            }

            return null; // hợp lệ
        }

        public async Task<ResponseDTO> CreateFarmActivityAsync(FarmActivityRequest farmActivityRequest, ActivityType activityType)
        {
            var validationResponse = await ValidateCreateAsync(farmActivityRequest, activityType);

            //Nếu validate fail → return ngay lỗi
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
                    //bool staffConflict = await _unitOfWork.farmActivityRepository.HasStaffTimeConflictAsync(
                    //    staffId: 3,
                    //    startDate: farmActivity.StartDate.Value,
                    //    endDate: farmActivity.EndDate.Value,
                    //    excludeActivityId: farmActivity.FarmActivitiesId);  // exclude chính nó

                    //if (staffConflict)
                    //{
                    //    return new ResponseDTO(Const.ERROR_EXCEPTION,
                    //        "Nhân viên được giao đã có hoạt động khác trùng thời gian trong khoảng này.");
                    //}
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

        public async Task<ResponseDTO> GetFarmActivitiesActiveAsync(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.farmActivityRepository.GetAllActive();

            if (list.IsNullOrEmpty())
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Not Found");
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
                //StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                //StaffEmail = fa.AssignedToNavigation?.Email,
                //StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                //StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
            }).ToList();

            var pagination = new Pagination<FarmActivityView>
            {
                TotalItemCount = views.Count(),
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = views.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };

            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagination);
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
                //StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                //StaffEmail = fa.AssignedToNavigation?.Email,
                //StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                //StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
            }).ToList();

            var pagination = new Pagination<FarmActivityView>
            {
                TotalItemCount = views.Count(),
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = views.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };

            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagination);
        }
        public async Task<ResponseDTO> GetFarmActivitiesByStaffAsync(int pageIndex, int pageSize, ActivityType? type, FarmActivityStatus? status, int? month)
        {
            var result = await _unitOfWork.farmActivityRepository.GetAllFiler(type, status, month);

            if (result.IsNullOrEmpty())
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Not Found");
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
                //StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                //StaffEmail = fa.AssignedToNavigation?.Email,
                //StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                //StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
            }).ToList();

            var pagination = new Pagination<FarmActivityView>
            {
                TotalItemCount = views.Count(),
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = views.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };

            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagination);
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
                //StaffId = (long)(fa.AssignedToNavigation?.AccountId),
                //StaffEmail = fa.AssignedToNavigation?.Email,
                //StaffFullName = fa.AssignedToNavigation?.AccountProfile?.Fullname,
                //StaffPhone = fa.AssignedToNavigation?.AccountProfile?.Phone
            };

            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, view);
        }


        public async Task<ResponseDTO> UpdateFarmActivityAsync(long farmActivityId, FarmActivityRequest farmActivityrequest, ActivityType activityType, FarmActivityStatus farmActivityStatus)
        {

            var user = await _jwtUtils.GetCurrentUserAsync();
            var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);

            if (farmActivity == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }

            var validation = await ValidateUpdateAsync(farmActivityId, farmActivityrequest, activityType, farmActivityStatus, farmActivity);
            if (validation != null) return validation;

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
            }
            //else if (farmActivity.ScheduleId == null)
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
            //  Chỉ staff được phân công mới được hoàn thành
            bool isAssigned = await _unitOfWork.staff_FarmActivityRepository
                .GetQueryable()
                .AnyAsync(sfa => sfa.FarmActivityId == farmActivity.FarmActivitiesId
                              && sfa.AccountId == user.AccountId);

            if (!isAssigned)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION,
                    "Bạn không được phân công cho hoạt động này. Chỉ nhân viên được phân công có quyền hoàn thành.");
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
                Notes = $"[Ghi chú tự động] Hoạt động {farmActivity.ActivityType} đã được {"chưa update *"} hoàn thành",
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
                foreach (var item in product)
                {
                    //CỘNG SL KHI THU HOẠCH
                    var schedule = await _unitOfWork.scheduleRepository.GetByCropId(item.ProductId);
                    item.StockQuantity += schedule.HarvestedQuantity;
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
            else if ((startDate.Value.DayNumber - endDate.Value.DayNumber) > 7)
            {
                return false;
            }
            return true;
        }
        public async Task<Response_DTO> AddStafftoFarmActivity(long farmActivityId, long staffId)
        {

            var validationResult = await ValidateAddStaffToFarmActivityAsync(farmActivityId, staffId);
            if (validationResult != null)
            {
                return validationResult;
            }

            var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);
            var staff = await _unitOfWork.accountRepository.GetByIdAsync(staffId);
            if (farmActivity == null || staff == null)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Not Found Farm Activity or Staff");
            }
            var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
            if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Người dùng không hợp lệ");
            }
            Staff_FarmActivity staff_FarmActivity = new Staff_FarmActivity
            {
                FarmActivityId = farmActivityId,
                AccountId = staffId,
                CreatedAt = DateTime.UtcNow,
                status = Status.ACTIVE,
                individualStatus = IndividualStatus.IN_PROGRESS,
                CreatedBy = (await _unitOfWork.accountProfileRepository.GetByIdAsync(getCurrentUser.AccountId))?.Fullname,
            };
            await _unitOfWork.staff_FarmActivityRepository.AddAsync(staff_FarmActivity);
            if (await _unitOfWork.SaveChangesAsync() < 0)
            {
                return new Response_DTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }
            else
            {
                var result = _mapper.Map<StaffFarmActivityResponse>(staff_FarmActivity);

                return new Response_DTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
            }
        }
        public async Task<Response_DTO> UpdateStafftoFarmActivity(long Staf_farmActivityId)//update status
        {

            //var validationResult = await ValidateUpdateStaffToFarmActivityAsync(Staf_farmActivityId);
            //if (validationResult != null)
            //{
            //    return validationResult;
            //}

            var getCurrentUser = await _jwtUtils.GetCurrentUserAsync();
            if (getCurrentUser == null || getCurrentUser.Role != Roles.Manager)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Người dùng không hợp lệ");
            }
            var staff_FarmActivity = await _unitOfWork.staff_FarmActivityRepository.GetByIdAsync(Staf_farmActivityId);
            if (staff_FarmActivity == null)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Not Found Staff_FarmActivity");
            }
            if (staff_FarmActivity.status.Equals(Status.ACTIVE))
            {
                staff_FarmActivity.status = Status.DEACTIVATED;
            }
            else
            {
                staff_FarmActivity.status = Status.ACTIVE;
            }
                staff_FarmActivity.UpdatedAt = DateTime.UtcNow;
            staff_FarmActivity.UpdatedBy = (await _unitOfWork.accountProfileRepository.GetByIdAsync(getCurrentUser.AccountId))?.Fullname;

            await _unitOfWork.staff_FarmActivityRepository.UpdateAsync(staff_FarmActivity);
            if (await _unitOfWork.SaveChangesAsync() < 0)
            {
                return new Response_DTO(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }
            else
            {
                var result = _mapper.Map<StaffFarmActivityResponse>(staff_FarmActivity);

                return new Response_DTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);
            }

        }
        public async Task<Response_DTO> GetAllFarmTask()
        {
            var staff_FarmActivity = await _unitOfWork.staff_FarmActivityRepository.GetAllAsync();
            if (staff_FarmActivity == null)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Not Found Staff_FarmActivity");
            }
            var result = _mapper.Map<List<StaffFarmActivityResponse>>(staff_FarmActivity);
            return new Response_DTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
        }
        public async Task<Response_DTO> GetFarmTaskById(long taskId)
        {
            var staff_FarmActivity = await _unitOfWork.staff_FarmActivityRepository.GetByIdAsync(taskId);
            if (staff_FarmActivity == null)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Not Found Staff_FarmActivity");
            }
            var result = _mapper.Map<StaffFarmActivityResponse>(staff_FarmActivity);
            return new Response_DTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
        }
        public async Task<Response_DTO> GetStaffByFarmActivityId(long farmActivityId)
        {
            var farmActivity = await _unitOfWork.staff_FarmActivityRepository.GetByFarmActivityIdAsync(farmActivityId);
            if (farmActivity == null)
            {
                return new Response_DTO(Const.FAIL_READ_CODE, "Not Found Farm Activity");
            }
            var result = _mapper.Map<List<StaffFarmActivityResponse>>(farmActivity);
            return new Response_DTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
        }

        public async Task<ResponseDTO> ReportMyPartCompletedAsync(long farmActivityId, string? notes = null)
        {
            var user = await _jwtUtils.GetCurrentUserAsync();
            if (user == null)
                return new ResponseDTO(Const.FAIL_READ_CODE, "Người dùng không hợp lệ.");

            // 1. Tìm assignment của user trong activity
            var assignment = await _unitOfWork.staff_FarmActivityRepository
                .GetQueryable()
                .FirstOrDefaultAsync(s => s.FarmActivityId == farmActivityId && s.AccountId == user.AccountId);

            if (assignment == null)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Bạn không được phân công cho hoạt động này.");

            if (assignment.status == Status.DEACTIVATED)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Phân công của bạn đã bị hủy.");

            // 2. Kiểm tra chưa hoàn thành
            if (assignment.individualStatus == IndividualStatus.COMPLETED)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Bạn đã báo hoàn thành phần việc rồi.");

            var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);
            if (farmActivity == null || farmActivity.Status == FarmActivityStatus.COMPLETED)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Hoạt động không tồn tại");

            var schedule = await _unitOfWork.scheduleRepository.GetScheduleByFarmActivityIdAsync(farmActivityId);
            if (schedule == null)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "lịch hoạt động không tồn tại");

            if (farmActivity.ActivityType == ActivityType.Harvesting)
            {
                if (!schedule.HarvestedQuantity.HasValue || schedule.HarvestedQuantity.Value <= 0)
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "Số lượng thu hoạch không hợp lệ.");
            }

            // 3. Cập nhật trạng thái cá nhân
            assignment.individualStatus = IndividualStatus.COMPLETED;
            assignment.UpdatedAt = DateTime.UtcNow;
            assignment.UpdatedBy = user.AccountId.ToString();

            await _unitOfWork.staff_FarmActivityRepository.UpdateAsync(assignment);

            if (farmActivity == null)
            {
                throw new Exception("FarmActivity not found");
            }

            // 4. Tạo log cá nhân
            var log = new ScheduleLog
            {
                FarmActivityId = farmActivityId,  //ID của hoạt động đang báo cáo hoàn thành

                ScheduleId = farmActivity.scheduleId ?? 0,  // Lấy từ entity FarmActivity

                Notes = $"[Ghi chú tự động] {user.AccountProfile?.Fullname ?? "Staff"} đã hoàn thành phần việc trong hoạt động {farmActivity.ActivityType}. Ghi chú: {notes ?? "Không có"}",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.AccountId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = user.AccountId  // Nếu UpdatedBy bắt buộc
            };
            await _scheduleLogRepo.AddAsync(log);

            // 5. Save trước khi check toàn bộ
            await _unitOfWork.SaveChangesAsync();

            // 6. Tự động kiểm tra và hoàn thành activity nếu tất cả staff xong
            await AutoCompleteActivityIfAllDoneAsync(farmActivityId);

            return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, "Bạn đã hoàn thành phần việc thành công.");
        }

        private async Task<ResponseDTO> AutoCompleteActivityIfAllDoneAsync(long farmActivityId)
        {
            // Load activity kèm list staff
            var farmActivity = await _unitOfWork.farmActivityRepository.GetByIdAsync(farmActivityId);
            if (farmActivity == null || farmActivity.Status == FarmActivityStatus.COMPLETED)
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Hoạt động không tồn tại");

            // Nếu không có staff nào được gán → không tự động hoàn thành
            if (!farmActivity.StaffFarmActivities.Any())
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Chưa có nhân viên được phân công trong hoạt động này");

            // Kiểm tra tất cả staff đã hoàn thành chưa
            bool allCompleted = farmActivity.StaffFarmActivities
                .All(sfa => sfa.individualStatus == IndividualStatus.COMPLETED);

            if (allCompleted)
            {
                // Cập nhật status activity
                farmActivity.Status = FarmActivityStatus.COMPLETED;
                farmActivity.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.farmActivityRepository.UpdateAsync(farmActivity);

                // Tạo log hệ thống
                var systemLog = new ScheduleLog
                {
                    FarmActivityId = farmActivity.FarmActivitiesId,
                    ScheduleId = farmActivity.scheduleId ?? 0,

                    Notes = $"[Ghi chú tự động] Hoạt động {farmActivity.ActivityType} đã hoàn thành khi tất cả nhân viên ({farmActivity.StaffFarmActivities.Count}) đã báo xong phần việc.",

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,

                    CreatedBy = farmActivity.createdBy,          // Hoặc ID user hệ thống nếu có
                    UpdatedBy = (long)farmActivity.updatedBy           // Tương tự
                };

                await _scheduleLogRepo.AddAsync(systemLog);

                // Xử lý Harvesting (cộng kho)
                if (farmActivity.ActivityType == ActivityType.Harvesting)
                {
                    var crops = await _unitOfWork.farmActivityRepository.GetCropsForHarvestActivity(farmActivity.FarmActivitiesId);
                    if (crops == null)
                    {
                        return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                    }
                    foreach (var crop in crops)
                    {
                        var product = crop.Product;
                        if (product == null) return new ResponseDTO(Const.ERROR_EXCEPTION, "Hạt giống hiện tại chưa có sản phẩm"); ;
                        //CỘNG SL KHI THU HOẠCH
                        var schedule = await _unitOfWork.scheduleRepository.GetActiveScheduleByCropId(crop.CropId);

                        double currentStock = product.StockQuantity ?? 0;
                        double harvested = schedule.HarvestedQuantity ?? 0;

                        product.StockQuantity = currentStock + harvested;

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
                else if(farmActivity.ActivityType == ActivityType.CleaningFarmArea)
                {
                    var currentSchedule = await _unitOfWork.scheduleRepository.GetScheduleByFarmActivityIdAsync(farmActivityId);
                    var allFarmActivity = await _unitOfWork.farmActivityRepository.GetListFarmActivityByScheduleId(currentSchedule.ScheduleId);
                    if (allFarmActivity == null)
                        return new ResponseDTO(Const.ERROR_EXCEPTION, "Không tìm thấy hoạt động trong lịch");
                    foreach (var activity in allFarmActivity)
                    {
                        if (activity.Status == FarmActivityStatus.ACTIVE && activity.FarmActivitiesId != farmActivityId)
                        {
                            systemLog = new ScheduleLog 
                            {
                                FarmActivityId = activity.FarmActivitiesId,
                                ScheduleId = activity.scheduleId ?? 0,

                                Notes = $"[Ghi chú tự động] Hoạt động {activity.ActivityType} đã chưa được hoàn thành trong lịch trình.",

                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,

                                CreatedBy = activity.createdBy,          // Hoặc ID user hệ thống nếu có
                                UpdatedBy = (long)activity.updatedBy           // Tương tự
                            };
                            await _unitOfWork.scheduleLogRepository.AddAsync(systemLog);
                        }
                    }
                    currentSchedule.Status = Status.COMPLETED;
                    await _unitOfWork.scheduleRepository.UpdateAsync(currentSchedule);
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

            return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, "Không có thay đổi nào được thực hiện.");
        }

        public async Task<ResponseDTO> GetFarmActivityByScheduleIdAsync(long scheduleId)
        {
            var scheduleExists = await _unitOfWork.scheduleRepository.GetByIdAsync(scheduleId);
            if (scheduleExists == null)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, "Không tìm thấy lịch trình với ID này.");
            }

            // Cách 1: Load full entity rồi map (dễ, phù hợp list nhỏ)
            var activities = await _farmActivityRepository.GetListFarmActivityByScheduleId(scheduleId);

            // AutoMapper map list
            var views = _mapper.Map<List<FarmActivityView>>(activities);

            return new ResponseDTO(Const.SUCCESS_READ_CODE,views.Any() ? Const.SUCCESS_READ_MSG : "Không có hoạt động nào.",views);
        }
    }
}
