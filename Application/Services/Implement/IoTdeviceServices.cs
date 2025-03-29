using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implement
{
    public class IoTdeviceServices : IIoTdeviceServices
    {
        public Task<IOTResponse.IOTView> CreateDeviceAsync(IOTResponse.IOTView device)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IOTResponse.IOTView>> GetAllDevices()
        {
            throw new NotImplementedException();
        }

        public Task<IOTResponse.IOTView> GetDeviceById(long deviceId)
        {
            throw new NotImplementedException();
        }

        public Task<IOTResponse.IOTView> GetInforInvironment(long deviceId)
        {
            throw new NotImplementedException();
        }

        public Task<IOTResponse.IOTView> UpdateDeviceAsync(long deviceId, IOTResponse.IOTView device)
        {
            throw new NotImplementedException();
        }

        public Task<IOTResponse.IOTView> UPdateStatusDeviceAsync(long deviceId)
        {
            throw new NotImplementedException();
        }
    }
}
