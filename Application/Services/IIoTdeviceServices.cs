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
        Task<IEnumerable<IOTView>> GetAllDevices();
        Task<IOTView> GetDeviceById(long deviceId);
        Task<IOTView> CreateDeviceAsync(IOTView device);
        Task<IOTView> UpdateDeviceAsync(long deviceId, IOTView device);
        Task<IOTView> UPdateStatusDeviceAsync(long deviceId);
        Task<IOTView> GetInforInvironment(long deviceId);

    }
}
