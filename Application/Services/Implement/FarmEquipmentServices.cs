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
                // 1. Validate input cơ bản (nên dùng FluentValidation hoặc check thủ công)
                if (request == null || request.deviceId <= 0 || request.FarmId <= 0)
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "Thiết bị hoặc nông trại không hợp lệ.");
                }

                // 2. Check thiết bị tồn tại
                var device = await _unitOfWork.deviceRepository.GetByIdAsync(request.deviceId);
                if (device == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Không tìm thấy thiết bị này.");
                }

                // 3. Check nông trại tồn tại
                var farm = await _unitOfWork.farmRepository.GetByIdAsync(request.FarmId);
                if (farm == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Không tìm thấy nông trại với ID này.");
                }

                // 4. Check thiết bị đã được gán ACTIVE trong nông trại này chưa
                var existingAssignment = await _unitOfWork.farmEquipmentRepository.GetAllAsync();
                var duplicate = existingAssignment.FirstOrDefault(x =>
                    x.DeviceId == request.deviceId &&
                    x.FarmId == request.FarmId && 
                    x.Status == Domain.Enum.Status.ACTIVE);

                if (duplicate != null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Thiết bị này đang được sử dụng trong nông trại này.");
                }

                // 5. Tạo FarmEquipment từ DTO
                var farmEquipment = _mapper.Map<Domain.Model.FarmEquipment>(request);
                // Mapper đã map FarmId rồi, nhưng để chắc chắn:
                farmEquipment.FarmId = request.FarmId;
                farmEquipment.AssignDate = _currentTime.GetCurrentTime();
                farmEquipment.Status = Domain.Enum.Status.ACTIVE;

                await _unitOfWork.farmEquipmentRepository.AddAsync(farmEquipment);
                await _unitOfWork.SaveChangesAsync();

                // 6. Tạo response view với tên Farm và Device
                var result = _mapper.Map<FarmEquipmentView>(farmEquipment);
                result.FarmName = farm.FarmName;
                result.DeviceName = device.DeviceName;

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, $"Lỗi hệ thống: {ex.Message}");
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

        public async Task<ResponseDTO> UpdateFarmEquipment(long farmEquipmentId, FarmEquipmentRequest request)
        {
            try
            {
                // 1. Validate input cơ bản
                if (request == null || request.deviceId <= 0 || request.FarmId <= 0)
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "Thiết bị hoặc nông trại không hợp lệ.");
                }

                // 2. Kiểm tra FarmEquipment tồn tại
                var existingEquipment = await _unitOfWork.farmEquipmentRepository.GetByIdAsync(farmEquipmentId);
                if (existingEquipment == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Không tìm thấy thiết bị nông trại với ID này.");
                }

                // 3. Kiểm tra thiết bị mới tồn tại
                var newDevice = await _unitOfWork.deviceRepository.GetByIdAsync(request.deviceId);
                if (newDevice == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Không tìm thấy thiết bị với ID này.");
                }

                // 4. Kiểm tra nông trại mới tồn tại
                var newFarm = await _unitOfWork.farmRepository.GetByIdAsync(request.FarmId);
                if (newFarm == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Không tìm thấy nông trại với ID này.");
                }

                // 5. Check trùng thiết bị ACTIVE trong nông trại mới (trừ chính bản ghi đang update)
                var allEquipments = await _unitOfWork.farmEquipmentRepository.GetAllAsync();
                var duplicate = allEquipments.FirstOrDefault(x =>
                    x.DeviceId == request.deviceId &&
                    x.FarmId == request.FarmId &&
                    x.Status == Domain.Enum.Status.ACTIVE &&
                    x.FarmEquipmentId != farmEquipmentId);  //Exclude chính bản ghi đang update

                if (duplicate != null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Thiết bị này đã được sử dụng trong nông trại này.");
                }

                // 6. Map dữ liệu từ request vào entity hiện có
                _mapper.Map(request, existingEquipment);  // AutoMapper update existing entity

                // Cập nhật các field cần thiết (nếu mapper chưa map hết)
                existingEquipment.FarmId = request.FarmId;
                existingEquipment.DeviceId = request.deviceId;  // Nếu request có thay đổi Device
                existingEquipment.Note = request.Note;          // Nếu có ghi chú
                existingEquipment.AssignDate = _currentTime.GetCurrentTime();  // Cập nhật ngày gán mới

                // 7. Cập nhật vào DB
                await _unitOfWork.farmEquipmentRepository.UpdateAsync(existingEquipment);
                await _unitOfWork.SaveChangesAsync();

                // 8. Tạo response view
                var result = _mapper.Map<FarmEquipmentView>(existingEquipment);
                result.FarmName = newFarm.FarmName;
                result.DeviceName = newDevice.DeviceName;

                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    } 
}
