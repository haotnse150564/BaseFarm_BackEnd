using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using Application.ViewModel.Request;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using static Application.ViewModel.Response.ProductResponse;
using static Infrastructure.ViewModel.Response.CropRequirementResponse;
using static Infrastructure.ViewModel.Response.CropResponse;
using static Infrastructure.ViewModel.Response.ScheduleResponse;
using ResponseDTO = Infrastructure.ViewModel.Response.CropResponse.ResponseDTO;

namespace Application.Services.Implement
{
    public class CropServices : ICropServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly ICropRepository _cropRepository;
        private readonly JWTUtils _jwtUtils;
        private readonly IHubContext<NotificationHub.ManagerNotificationHub> _hubContext;
        public CropServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, ICropRepository cropRepository, JWTUtils jWTUtils, IHubContext<NotificationHub.ManagerNotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _cropRepository = cropRepository;
            _jwtUtils = jWTUtils;
            _hubContext = hubContext;
        }
        #region Get Crop All requirements
        public async Task<Pagination<CropView>> GetAllCropsAsync(int pageIndex, int pageSize)
        {
            try
            {
                var crop = await _unitOfWork.cropRepository.GetAllAsync();
                if (crop.IsNullOrEmpty())
                {
                    throw new Exception();
                }
                else
                {
                    // Map dữ liệu sang DTO
                    var result = _mapper.Map<List<CropView>>(crop);
                    var pagination = new Pagination<CropView>
                    {
                        TotalItemCount = result.Count,
                        PageSize = pageSize,
                        PageIndex = pageIndex,
                        Items = result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                    };
                    return pagination;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<CropView>> GetAllCropsActiveAsync()
        {
            var result = await _unitOfWork.cropRepository.GetAllAsync();
            var cropFilter = result.Where(x => x.Status == Domain.Enum.CropStatus.ACTIVE).ToList();
            if (cropFilter.IsNullOrEmpty())
            {
                throw new Exception();
            }
            else
            {
                // Map dữ liệu sang DTO
                var resultView = _mapper.Map<List<CropView>>(cropFilter);
                return resultView;
            }
        }

        public async Task<ResponseDTO> CreateCropAsync(CropRequest request, ProductRequestDTO.CreateProductDTO product)
        {
            try
            {
               var currentUser = await _jwtUtils.GetCurrentUserAsync();
                if (currentUser == null || currentUser.Role != Roles.Manager)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
                }

                var products = _mapper.Map<Product>(product);
                products.CreatedAt = _currentTime.GetCurrentTime();
                products.StockQuantity = 0;
                products.Status = ProductStatus.ACTIVE;
                products.CategoryId = request.CategoryId;
                await _unitOfWork.productRepository.AddAsync(products);
               await _unitOfWork.SaveChangesAsync();

                var crop = _mapper.Map<Crop>(request);
                crop.CropId = products.ProductId;
                crop.CreateAt = _currentTime.GetCurrentTime();
                crop.UpdateAt = _currentTime.GetCurrentTime();
                await _unitOfWork.cropRepository.AddAsync(crop);

                await _unitOfWork.SaveChangesAsync();

                var category = await _unitOfWork.categoryRepository.GetByIdAsync((long)crop.CategoryId);

                var result1 = _mapper.Map<CropView>(crop);
                var result2 = _mapper.Map<ProductDetailDTO>(products);

                result2.CategoryName = category.CategoryName;
                result2.CropName = crop.CropName;

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, new { Crop = result1, Product = result2 });
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateCropStatusAsync(long cropId, int status)
        {
            try
            {
                var crop = await _unitOfWork.cropRepository.GetByIdAsync(cropId);
                if (crop == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "crop not found !");
                }

                crop.Status = (CropStatus)status;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.cropRepository.UpdateAsync(crop);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_UPDATE_MSG, "Change Status Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }

        }

        public async Task<ResponseDTO> SearchCrop(string? cropName, Status? status, int pageIndex, int pageSize)
        {
            try
            {
                var crop = await _unitOfWork.cropRepository.GetAllAsync();
                if (crop == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "crop not found !");
                }
                else if (cropName != null)
                {
                    crop = crop.Where(x =>  x.CropName.ToLower().Contains(cropName.ToLower())).ToList();
                }
                else if (status != null)
                {
                    crop = crop.Where(x => x.Status.Equals(status)).ToList();
                }

                var result = _mapper.Map<List<CropView>>(crop);
                var pagination = new Pagination<CropView>
                {
                    TotalItemCount = result.Count,
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

        public async Task<ResponseDTO> UpdateCrop(CropRequest cropUpdate, long cropId)
        {
            var crop = await _unitOfWork.cropRepository.GetByIdAsync(cropId);
            try
            {
                if (crop == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Crop not found !");
                }
                var crops = _mapper.Map(cropUpdate, crop);
                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.cropRepository.UpdateAsync(crops);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.Map<CropView>(crops);
                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetCropExcludingInativeAsync()
        {
            var crop = await _unitOfWork.cropRepository.GetAllexcludingInactiveAsync();
            if (crop.IsNullOrEmpty())
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "No crops found.");
            }
            else
            {
                var result = _mapper.Map<List<CropView>>(crop);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
        }

        public async Task<Dictionary<string, object>> CheckTemperatureAlertsAsync()
        {
            var currentUser = await _jwtUtils.GetCurrentUserAsync();
            if (currentUser == null || currentUser.Role != Roles.Manager)
            {
                throw new Exception("Chưa Login hoặc role không phải là Manager");
            }
            var result = new Dictionary<string, object>
            {
                { "Timestamp", DateTime.Now },
                { "AlertsSent", 0 },
                { "ActiveSchedules", 0 }
            };

            try
            {
                // Lấy tất cả Schedule active
                var activeSchedules = await _unitOfWork.scheduleRepository.GetAllActiveScheduleAsync();
                result["ActiveSchedules"] = activeSchedules.Count;

                // Lấy nhiệt độ mới nhất từ DHT11 (V0)
                var latestTempLog = await _unitOfWork.iotLogRepository.GetLatestByPinAsync("V0");
                if (latestTempLog == null)
                {
                    result["Message"] = "Không có dữ liệu nhiệt độ mới nhất";
                    return result;
                }

                decimal currentTemp = (decimal)latestTempLog.Value;
                result["CurrentTemperature"] = currentTemp;

                int alertsSent = 0;

                foreach (var schedule in activeSchedules)
                {
                    var requirement = schedule.Crop.CropRequirement
                        .FirstOrDefault(r => r.PlantStage == schedule.currentPlantStage);

                    if (requirement == null) continue;

                    bool tempAlert = requirement.Temperature.HasValue &&
                        (currentTemp < requirement.Temperature.Value - 3 ||
                         currentTemp > requirement.Temperature.Value + 3);

                    if (tempAlert)
                    {
                        string message = $"Cảnh báo môi trường cho cây {schedule.Crop.CropName} (Lịch {schedule.ScheduleId}):\n" +
                                         $"Nhiệt độ hiện tại: {currentTemp}°C (Yêu cầu: {requirement.Temperature}°C)";

                        await _hubContext.Clients.Group($"User_{currentUser.AccountId}")
                                         .SendAsync("ReceiveNotification", message);

                        alertsSent++;
                    }
                }

                result["AlertsSent"] = alertsSent;
                result["Message"] = alertsSent > 0 ? $"Đã gửi {alertsSent} cảnh báo" : "Không có cảnh báo";
            }
            catch (Exception ex)
            {
                result["Error"] = ex.Message;
            }

            return result;
        }
        #endregion

    }
}
