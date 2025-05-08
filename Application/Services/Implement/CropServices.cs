using Application.Interfaces;
using AutoMapper;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.ViewModel.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static Infrastructure.ViewModel.Response.CropResponse;
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
        public async Task<List<CropView>> GetAllCropsAsync()
        {
            var result = await _unitOfWork.cropRepository.GetAllAsync();
            if (result.IsNullOrEmpty())
            {
                throw new Exception();
            }
            else
            {
                // Map dữ liệu sang DTO
                var resultView = _mapper.Map<List<CropView>>(result);
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
                crop.Status = Domain.Enum.Status.ACTIVE;

                // Gọi AddAsync nhưng không gán vào biến vì nó không có giá trị trả về
                await _unitOfWork.cropRepository.AddAsync(crop);
                var check = await _unitOfWork.SaveChangesAsync();
                // Kiểm tra xem sản phẩm có được thêm không bằng cách kiểm tra product.Id (hoặc khóa chính)
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
    }
}
