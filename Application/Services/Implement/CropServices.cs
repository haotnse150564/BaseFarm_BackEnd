using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using Application.ViewModel.Request;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.ViewModel.Request;
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
        public CropServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, ICropRepository cropRepository, JWTUtils jWTUtils)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _cropRepository = cropRepository;
            _jwtUtils = jWTUtils;
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
        #endregion
        #region Get all Crop only 1 crop requirements
        public async Task<Pagination<CropView>> Get_AllCropsAsync(int pageIndex, int pageSize)
        {
            try
            {
                var crops = await _unitOfWork.cropRepository.GetAllAsync();
                if (crops == null || !crops.Any())
                {
                    throw new Exception("Không có dữ liệu");
                }

                var cropViews = new List<CropView>();

                foreach (var crop in crops)
                {
                    // Xác định PlantStage hiện tại từ Schedule → FarmActivity
                    var currentStage = crop.Schedules?
                        .OrderByDescending(s => s.StartDate) // hoặc logic chọn schedule hiện tại
                        .FirstOrDefault()?.FarmActivities?.PlantStage;

                    // Lấy requirement tương ứng với stage
                    var requirement = crop.CropRequirement?   // <-- sửa thành CropRequirements
                        .FirstOrDefault(r => r.PlantStage == currentStage);

                    // Map sang CropView
                    var cropView = _mapper.Map<CropView>(crop);

                    // Chỉ giữ requirement hiện tại
                    cropView.CropRequirements = requirement != null
                        ? new List<CropRequirementView> { _mapper.Map<CropRequirementView>(requirement) }
                        : new List<CropRequirementView>();

                    cropViews.Add(cropView);
                }

                var pagination = new Pagination<CropView>
                {
                    TotalItemCount = cropViews.Count,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = cropViews.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<CropView>> Get_AllCropsActiveAsync()
        {
            var crops = await _unitOfWork.cropRepository.GetAllAsync();
            var cropFilter = crops.Where(x => x.Status == Domain.Enum.CropStatus.ACTIVE).ToList();

            if (cropFilter == null || !cropFilter.Any())
            {
                throw new Exception("Không có crop nào ACTIVE");
            }

            var cropViews = new List<CropView>();

            foreach (var crop in cropFilter)
            {
                // Lấy PlantStage hiện tại từ Schedule → FarmActivity
                var currentStage = crop.Schedules?
                    .OrderByDescending(s => s.StartDate) // hoặc logic chọn schedule hiện tại
                    .FirstOrDefault()?.FarmActivities?.PlantStage;

                // Lấy requirement tương ứng với stage (dùng CropRequirements - collection)
                var requirement = crop.CropRequirement?
                    .FirstOrDefault(r => r.PlantStage == currentStage);

                // Map sang CropView
                var cropView = _mapper.Map<CropView>(crop);

                // Chỉ giữ requirement hiện tại
                cropView.CropRequirements = requirement != null
                    ? new List<CropRequirementView> { _mapper.Map<CropRequirementView>(requirement) }
                    : new List<CropRequirementView>();

                cropViews.Add(cropView);
            }

            return cropViews;
        }
        public async Task<ResponseDTO> Search_Crop(string? cropName, CropStatus? status, int pageIndex, int pageSize)
        {
            try
            {
                var crops = await _unitOfWork.cropRepository.GetAllAsync();
                if (crops == null || !crops.Any())
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "crop not found !");
                }

                // Lọc theo cropName nếu có
                if (!string.IsNullOrEmpty(cropName))
                {
                    crops = crops.Where(x => x.CropName != null &&
                                             x.CropName.ToLower().Contains(cropName.ToLower()))
                                 .ToList();
                }

                // Lọc theo status nếu có
                if (status != null)
                {
                    crops = crops.Where(x => x.Status == status).ToList();
                }

                // Xử lý requirement theo PlantStage hiện tại
                var cropViews = new List<CropView>();
                foreach (var crop in crops)
                {
                    var currentStage = crop.Schedules?
                        .OrderByDescending(s => s.StartDate)
                        .FirstOrDefault()?.FarmActivities?.PlantStage;

                    var requirement = crop.CropRequirement?
                        .FirstOrDefault(r => r.PlantStage == currentStage);

                    var cropView = _mapper.Map<CropView>(crop);
                    cropView.CropRequirements = requirement != null
                        ? new List<CropRequirementView> { _mapper.Map<CropRequirementView>(requirement) }
                        : new List<CropRequirementView>();

                    cropViews.Add(cropView);
                }

                var pagination = new Pagination<CropView>
                {
                    TotalItemCount = cropViews.Count,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = cropViews.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagination);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<ResponseDTO> GetCropExcludingInactiveAsync()
        {
            var crops = await _unitOfWork.cropRepository.GetAllexcludingInactiveAsync();
            if (crops == null || !crops.Any())
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "No crops found.");
            }

            var cropViews = new List<CropView>();

            foreach (var crop in crops)
            {
                // Lấy PlantStage hiện tại từ Schedule → FarmActivity
                var currentStage = crop.Schedules?
                    .OrderByDescending(s => s.StartDate) // hoặc logic chọn schedule hiện tại
                    .FirstOrDefault()?.FarmActivities?.PlantStage;

                // Lấy requirement tương ứng với stage
                var requirement = crop.CropRequirement?
                    .FirstOrDefault(r => r.PlantStage == currentStage);

                // Map sang CropView
                var cropView = _mapper.Map<CropView>(crop);

                // Chỉ giữ requirement hiện tại
                cropView.CropRequirements = requirement != null
                    ? new List<CropRequirementView> { _mapper.Map<CropRequirementView>(requirement) }
                    : new List<CropRequirementView>();

                cropViews.Add(cropView);
            }

            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, cropViews);
        }
        #endregion
    }
}
