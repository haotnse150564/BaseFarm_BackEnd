using Infrastructure.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.IOTResponse;

namespace Application.Services
{
    public interface IIoTdeviceServices
    {
        Task<ResponseDTO> GetAllDevices(int pageIndex, int pageSize);
        Task<ResponseDTO> GetDeviceById(long deviceId);
        Task<ResponseDTO> CreateDeviceAsync(IOTRequest device);
        Task<ResponseDTO> UpdateDeviceAsync(long deviceId, IOTRequest device);
        Task<ResponseDTO> UpdateStatusDeviceAsync(long deviceId, string status);
        Task<ResponseDTO> GetInforInvironment(long deviceId);

    }
}
