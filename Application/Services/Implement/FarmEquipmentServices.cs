using Application.Interfaces;
using AutoMapper;
using Infrastructure.Repositories;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.FarmEquipmentResponse;


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

        public async Task<ResponseDTO> CreateFarmEquipment(FarmEquipmentRequest request)
        {
            try
            {
                var checklistEquipments = await _unitOfWork.farmEquipmentRepository.GetAllAsync();
                if (checklistEquipments.Any())
                {
                    var checkExist = checklistEquipments.FirstOrDefault(x => x.DeviceId == request.deviceId && x.FarmId == 1 && x.Status == Domain.Enum.Status.ACTIVE);
                    if (checkExist != null)
                    {
                        return new ResponseDTO(Const.FAIL_READ_CODE, "Thiết bị đang được sử dụng");
                    }
                }
                var checkDevice = await _unitOfWork.deviceRepository.GetByIdAsync(request.deviceId);
                if(checkDevice == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Không có thiết bị này");
                }
                var fe = _mapper.Map<Domain.Model.FarmEquipment>(request);
                fe.FarmId = 1; //Default farm id
                fe.AssignDate = _currentTime.GetCurrentTime();
                fe.Status = Domain.Enum.Status.ACTIVE;

                await _unitOfWork.farmEquipmentRepository.AddAsync(fe);
                await _unitOfWork.SaveChangesAsync();

                var farm = await _unitOfWork.farmRepository.GetByIdAsync(1);
                var device = await _unitOfWork.deviceRepository.GetByIdAsync(request.deviceId);
                var result = _mapper.Map<FarmEquipmentView>(fe);
                result.FarmName = farm.FarmName;
                result.DeviceName = device.DeviceName;

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<ResponseDTO> GetAll()
        {
            try
            {
                var list = await _unitOfWork.farmEquipmentRepository.GetAllAsync();
                var result = _mapper.Map<List<FarmEquipmentView>>(list);

                if (result == null || result.Count == 0)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }

        }

        public async Task<ResponseDTO> GetFarmEquipmentByDevicesName(string name)
        {
            try
            {
                var list = await _unitOfWork.farmEquipmentRepository.GetFarmEquipmentByDeviceName(name);
                var result = _mapper.Map<List<FarmEquipmentView>>(list);

                if (result == null || result.Count == 0)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetListEquipmentActive()
        {
            try
            {
                var list = await _unitOfWork.farmEquipmentRepository.GetFarmEquipmentActive();
                var result = _mapper.Map<List<FarmEquipmentView>>(list);

                if (result == null || result.Count == 0)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> RemmoveFarmEquipment(long id)
        {
            try
            {
                var result = await _unitOfWork.farmEquipmentRepository.GetByIdAsync(id);
                if (result == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                result.Status = Domain.Enum.Status.DEACTIVATED;
                _unitOfWork.farmEquipmentRepository.Update(result);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    } 
}
