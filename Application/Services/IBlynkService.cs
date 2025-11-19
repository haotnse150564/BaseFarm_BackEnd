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

        /// <summary>
        /// Cập nhật ngưỡng LDR Thấp (V10) – khi trời tối
        /// </summary>
        Task<bool> SetLdrLowThresholdAsync(int value);

        /// <summary>
        /// Cập nhật ngưỡng LDR Cao (V11) – khi trời sáng
        /// </summary>
        Task<bool> SetLdrHighThresholdAsync(int value);

        /// <summary>
        /// Cập nhật ngưỡng độ ẩm đất để BẬT bơm (V12)
        /// </summary>
        Task<bool> SetSoilOnThresholdAsync(int value);

        /// <summary>
        /// Cập nhật ngưỡng độ ẩm đất để TẮT bơm (V13)
        /// </summary>
        Task<bool> SetSoilOffThresholdAsync(int value);
    }
}
