using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IBlynkService
    {
        Task<Dictionary<string, object?>> GetAllDatastreamValuesAsync();
        Task<bool> ControlPumpAsync(bool isOn);
        Task<bool> ControlServoAsync(int angle);
        Task<bool> SetManualModeAsync(bool isManual);
    }
}
