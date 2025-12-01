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
        Task<ResponseDTO> GetList(int pageNum = 1);
        Task<string> UpdateLogAsync(Dictionary<string, object?> obj);
        Task<byte[]> ExportLogsToCsvAsync();

        // ==================== ĐÈN LED (GROW LIGHT) ====================
        /// <summary>
        /// Bật/tắt đèn thủ công (V12) - Chỉ có hiệu lực khi hệ thống ở chế độ Manual (V7 = 1)
        /// </summary>
        Task<bool> ControlLightAsync(bool isOn);

        /// <summary>
        /// Ngưỡng bật đèn khi trời tối (V13) - LDR <= giá trị này → bật đèn (khi ở Auto)
        /// </summary>
        Task<bool> SetLightOnThresholdAsync(int value);

        /// <summary>
        /// Ngưỡng tắt đèn khi trời sáng (V14) - LDR >= giá trị này → tắt đèn (khi ở Auto)
        /// </summary>
        Task<bool> SetLightOffThresholdAsync(int value);
    }
}
