using Application.Commons;
using Application.Interfaces;
using AutoMapper;
using Azure.Core;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using Microsoft.Extensions.Configuration;
using System.Drawing.Printing;
using static Infrastructure.ViewModel.Response.IOTResponse;
using static Infrastructure.ViewModel.Response.ScheduleResponse;
using ResponseDTO = Infrastructure.ViewModel.Response.IOTResponse.ResponseDTO;

namespace Application.Services.Implement
{
    public class IoTdeviceServices : IIoTdeviceServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        public IoTdeviceServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
        }
        public async Task<ResponseDTO> CreateDeviceAsync(IOTRequest request)
        {
            try
            {
                var result = _mapper.Map<IoTdevice>(request);
                result.Status = (int?)(int?)Status.ACTIVE; // Gán trạng thái mặc định là Active
                result.LastUpdate = _currentTime.GetCurrentTime(); // Cập nhật thời gian hiện tại
                // Map dữ liệu sang DTO
                await _unitOfWork.ioTdeviceRepository.AddAsync(result);
                await _unitOfWork.SaveChangesAsync();

                var resultView = _mapper.Map<IOTView>(result);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, resultView);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetAllDevices(int pageIndex, int pageSize)
        {

            try
            {
                var devices = await _unitOfWork.ioTdeviceRepository.GetAllAsync();

                if (devices == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No IOT found.");
                }

                // Map dữ liệu sang DTO
                var result = _mapper.Map<List<IOTView>>(devices);
                var totalItem = result.Count(); // Lấy tổng số bản ghi
                var pagination = new Pagination<IOTView>
                {
                    TotalItemCount = totalItem,
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

        public async Task<ResponseDTO> GetDeviceById(long deviceId)
        {
            try
            {
                var devices = await _unitOfWork.ioTdeviceRepository.GetByIdAsync(deviceId);
                if (devices == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No IOT found.");
                }

                // Map dữ liệu sang DTO
                var result = _mapper.Map<IOTView>(devices);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public Task<ResponseDTO> GetInforInvironment(long deviceId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO> UpdateDeviceAsync(long deviceId, IOTRequest device)
        {
            try
            {
                var ioTdevice = await _unitOfWork.ioTdeviceRepository.GetByIdAsync(deviceId);

                if (ioTdevice == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Device found.");
                }
                var update = _mapper.Map(device, ioTdevice);
                update.LastUpdate = _currentTime.GetCurrentTime(); // Cập nhật thời gian hiện tại
                // Map dữ liệu sang DTO
                _unitOfWork.ioTdeviceRepository.Update(update);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.Map<IOTView>(update);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
            
        }

        public async Task<ResponseDTO> UpdateStatusDeviceAsync(long deviceId, string status)
        {
            try
            {
                var ioTdevice = await _unitOfWork.ioTdeviceRepository.GetByIdAsync(deviceId);

                if (ioTdevice == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Device found.");
                }

                // Map dữ liệu sang DTO
                ioTdevice.Status = (int?)(Status)Enum.Parse(typeof(Status), status); // Chuyển chuỗi sang Enum
                await _unitOfWork.ioTdeviceRepository.UpdateAsync(ioTdevice);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<IOTView>(ioTdevice);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
