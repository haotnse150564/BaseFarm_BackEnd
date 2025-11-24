using Application.Interfaces;
using AutoMapper;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implement
{

    public class FarmEquipmentServices : IFarmEquipmentServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly IFarmEquipmentRepository _farmEquipmentRepository;

        public FarmEquipmentServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
        }

        public Task<object> CreateFarmEquipment(List<int> listId)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<object> GetFarmEquipmentByDevicesName(long farmId)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetListEquipmentByFarmId(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemmoveFarmEquipment(long id)
        {
            throw new NotImplementedException();
        }
    }
}
