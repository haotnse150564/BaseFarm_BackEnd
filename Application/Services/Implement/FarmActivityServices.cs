﻿using Application.Interfaces;
using Application;
using Application.Services;
using AutoMapper;
using Infrastructure.ViewModel.Response;
using Microsoft.Extensions.Configuration;
using Infrastructure.Repositories;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Services
{
    public class FarmActivityServices : IFarmActivityServices

    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly IFarmActivityRepository _farmActivityRepository;
        public FarmActivityServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, IFarmActivityRepository farmActivityRepository)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _farmActivityRepository = farmActivityRepository;
        }
        public async Task<List<FarmActivityView>> GetFarmActivitiesAsync()
        {
            var result = await _unitOfWork.farmActivityRepository.GetAllAsync();
            if (result.IsNullOrEmpty())
            {
                throw new Exception();
            }
            else
            {
                // Map dữ liệu sang DTO
                var resultView = _mapper.Map<List<FarmActivityView>>(result);
                return resultView;
            }
        }
    }
}
