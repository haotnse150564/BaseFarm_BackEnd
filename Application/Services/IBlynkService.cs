using static Infrastructure.ViewModel.Response.IOTLogResponse;

namespace Application.Services
{
    public interface IBlynkService
    {
        Task<Dictionary<string, object?>> GetAllDatastreamValuesAsync();
        Task<bool> ControlPumpAsync(bool isOn);
        Task<bool> ControlServoAsync(int angle);
        Task<bool> SetManualModeAsync(bool isManual);

        /// <summary>
        /// V8 - Độ Ẩm Đất Thấp → BẬT bơm khi soil ≤ value
        /// </summary>
        Task<bool> SetSoilLowThresholdAsync(int value);

        /// <summary>
        /// V9 - Độ Ẩm Đất Cao → TẮT bơm khi soil ≥ value
        /// </summary>
        Task<bool> SetSoilHighThresholdAsync(int value);

        /// <summary>
        /// V10 - Ánh Sáng Ngưỡng Thấp → servo 0° khi LDR ≤ value
        /// </summary>
        Task<bool> SetLdrLowThresholdAsync(int value);

        /// <summary>
        /// V11 - Ánh Sáng Ngưỡng Cao → servo 180° khi LDR ≥ value
        /// </summary>
        Task<bool> SetLdrHighThresholdAsync(int value);
        Task<ResponseDTO> GetList();
        Task<string> UpdateLogAsync();
    }
}
