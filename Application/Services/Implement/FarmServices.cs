using Application.Interfaces;
using Application;
using Application.Services;
using AutoMapper;
using Infrastructure.ViewModel.Response;
using Microsoft.Extensions.Configuration;
using static Infrastructure.ViewModel.Response.FarmDetailResponse;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Repositories;

namespace WebAPI.Services
{
    public class FarmServices : IFarmServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly IFarmRepository _farmRepository;
        public FarmServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, IFarmRepository farmRepository)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _farmRepository = farmRepository;
        }
        public async Task<List<FarmDetailView>> GetAll()
        {
            var result = await _unitOfWork.farmRepository.GetAllAsync();

            if (result.IsNullOrEmpty())
            {
                throw new Exception();
            }
            else
            {
                // Map dữ liệu sang DTO
                var resultView = _mapper.Map<List<FarmDetailResponse.FarmDetailView>>(result);
                return resultView;
            }
        }
    }
}
