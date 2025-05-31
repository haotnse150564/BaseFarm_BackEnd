using Application.Commons;
using Application.Interfaces;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.ViewModel.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
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
        public CropServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, ICropRepository cropRepository)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _cropRepository = cropRepository;
        }
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

        public async Task<ResponseDTO> CreateCropAsync(CropRequest request)
        {
            try
            {
                //if (await _unitOfWork.cropRepository.CheckDuplicateCropName(request.CropName))
                //{
                //    return new ResponseDTO(Const.FAIL_CREATE_CODE, "The Crop Name already exists. Please choose a different Crop Name.");
                //}

                // Ánh xạ từ DTO sang Entity
                var crop = _mapper.Map<Crop>(request);
                crop.Status = Domain.Enum.CropStatus.ACTIVE;
                var cropRequirement = new CropRequirement
                {
                    RequirementId = crop.CropId,
                    EstimatedDate = 30,
                    Moisture = 1,
                    Temperature = 30,
                    Fertilizer = "Nito",
                    DeviceId = 4,
                    Requirement = crop // Thiết lập quan hệ với Crop
                };

                // Gọi AddAsync nhưng không gán vào biến vì nó không có giá trị trả về
                await _unitOfWork.cropRepository.AddAsync(crop);
                await _unitOfWork.cropRequirementRepository.AddAsync(cropRequirement);

                var check = await _unitOfWork.SaveChangesAsync();
                // Kiểm tra xem sản phẩm có được thêm không bằng cách kiểm tra crop.Id (hoặc khóa chính)
                if (check < 0) // Nếu Id chưa được gán, có thể việc thêm đã thất bại
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Failed to add crop.");
                }

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Crop Create successfully");
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
                    crop = crop.Where(x => x.CropName.ToLower().Contains(cropName.ToLower())).ToList();
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
    }
}
