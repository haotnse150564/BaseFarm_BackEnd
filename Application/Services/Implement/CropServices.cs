using Application.Interfaces;
using AutoMapper;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static Infrastructure.ViewModel.Response.CropResponse;

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
    }
}
