using AutoMapper;
using Domain.Model;
using Infrastructure;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.IOTLogResponse;

namespace Application.Services.Implement
{
    public class BlynkService : IBlynkService
    {
        private readonly HttpClient _httpClient;
        private const string BlynkToken = "xRd0sDuPYqFjPI1ZSHRr7Bd1cJq3fH2Y";
        private const string BlynkBaseUrl = "https://sgp1.blynk.cloud";
        private const string BlynkWriteBaseUrl = "https://sgp1.blynk.cloud/external/api";
        private readonly IUnitOfWorks _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Timer _timer;
        public BlynkService(HttpClient httpClient, IUnitOfWorks unitOfWork, IMapper mapper, Timer timer)
        {
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timer = new Timer(async _ =>
            {
                await UpdateLogAsync();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

        }

        public async Task<Dictionary<string, object?>> GetAllDatastreamValuesAsync()
        {
            try
            {
                var url = $"{BlynkBaseUrl}/external/api/getAll?token={BlynkToken}";
                Console.WriteLine($"Fetching all Datastreams from URL: {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching all datastreams: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                // Chuyển đổi JsonElement sang object
                var result = new Dictionary<string, object?>();
                foreach (var kvp in data)
                {
                    result[kvp.Key] = ConvertJsonElementToString(kvp.Value);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        // Hàm chuyển đổi JsonElement sang chuỗi
        private string? ConvertJsonElementToString(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => null,
            };
        }

        /// <summary>
        /// Gửi giá trị đến Blynk datastream (Virtual Pin)
        /// </summary>
        private async Task<bool> SendCommandAsync(string pin, string value)
        {
            var url = $"{BlynkWriteBaseUrl}/update?token={BlynkToken}&{pin}={value}";
            var response = await _httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Bật/tắt máy bơm (V5)
        /// </summary>
        public async Task<bool> ControlPumpAsync(bool isOn)
        {
            string value = isOn ? "1" : "0";
            return await SendCommandAsync("V5", value);
        }

        /// <summary>
        /// Điều chỉnh góc servo (V6)
        /// </summary>
        public async Task<bool> ControlServoAsync(int angle)
        {
            if (angle < 0) angle = 0;
            if (angle > 180) angle = 180;
            return await SendCommandAsync("V6", angle.ToString());
        }

        /// <summary>
        /// Tự Động hoặc điều khiển bằng tay (V7)
        /// </summary>
        public async Task<bool> SetManualModeAsync(bool isManual)
        {
            // V7 là pin điều khiển chế độ Manual / Auto
            string value = isManual ? "1" : "0";
            return await SendCommandAsync("V7", value);
        }

        // Thay 4 method cũ bằng 4 method mới này
        public async Task<bool> SetSoilLowThresholdAsync(int value)
        {
            if (value < 0 || value > 100) return false;
            return await SendCommandAsync("V8", value.ToString());
        }

        public async Task<bool> SetSoilHighThresholdAsync(int value)
        {
            if (value < 0 || value > 100) return false;
            return await SendCommandAsync("V9", value.ToString());
        }

        public async Task<bool> SetLdrLowThresholdAsync(int value)
        {
            if (value < 0 || value > 1023) return false;
            return await SendCommandAsync("V10", value.ToString());
        }

        public async Task<bool> SetLdrHighThresholdAsync(int value)
        {
            if (value < 0 || value > 1023) return false;
            return await SendCommandAsync("V11", value.ToString());
        }

        /// <summary>
        /// LOG dữ liệu từ Blynk về database
        /// </summary>
        /// <returns></returns>
        public async Task<string> UpdateLogAsync()
        {
            try
            {
                var url = $"{BlynkBaseUrl}/external/api/getAll?token={BlynkToken}";
                Console.WriteLine($"Fetching all Datastreams from URL: {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching all datastreams: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                // Chuyển đổi JsonElement sang object
                var result = new Dictionary<string, object?>();
                foreach (var kvp in data)
                {
                    result[kvp.Key] = ConvertJsonElementToString(kvp.Value);
                }

                var sortedResult = result
                                   .OrderBy(kvp => int
                                   .Parse(kvp.Key.Substring(1))) // lấy phần số sau chữ V
                                   .ToList();

                string[] DeviceName = ["Nhiet Do", "Do Am", "Luong Mua", "Do Am Dat", "Anh Sang"];
                List<LogEntry> logEntries = new List<LogEntry>();
                for (int i = 0; i < 5; i++)
                {
                    var entry = sortedResult[i];
                    LogEntry logEntry = new LogEntry
                    {
                        Pin = entry.Key,
                        Value = double.Parse(entry.Value.ToString() ?? "0"),
                        DeviceName = DeviceName[i]
                    };
                    logEntries.Add(logEntry);
                }
                foreach (var log in logEntries)
                {
                    var divces = await _unitOfWork.deviceRepository.GetDevieByName(log.DeviceName);
                    if (divces == null)
                    {
                        continue; // Hoặc xử lý lỗi tùy theo yêu cầu của bạn
                    }

                    IOTLog iotLog = new IOTLog
                    {
                        DevicesId = divces.DevicesId,
                        VariableId = log.Pin,
                        SensorName = log.DeviceName,
                        Value = log.Value,
                        Timestamp = DateTime.Now
                    };

                    var listIotLog = await _unitOfWork.iotLogRepository.GetAllAsync();
                    // Sắp xếp giảm dần theo thời gian tạo
                    int deleteCount = 200;

                    var sortedList = listIotLog.OrderByDescending(x => x.Timestamp).ToList();
                    if (sortedList.Count > deleteCount)
                    {
                        var toDelete = sortedList.Skip(deleteCount).ToList(); // lấy những item từ vị trí 201 trở đi

                        foreach (var item in toDelete)
                        {
                            await _unitOfWork.iotLogRepository.DeleteAsync(item);
                            await _unitOfWork.SaveChangesAsync();
                        }

                        // Giữ lại 20 item mới nhất
                        sortedList = sortedList.Take(deleteCount).ToList();

                    }
                    await _unitOfWork.iotLogRepository.AddAsync(iotLog);
                    await _unitOfWork.SaveChangesAsync();
                }
                return "Log đã được cập nhật";
            }
            catch (Exception ex)
            {
                return "Có lỗi xảy ra";
            }
        }
        class LogEntry
        {
            public string Pin { get; set; }
            public double Value { get; set; }
            public string DeviceName { get; set; }
        }
        public async Task<ResponseDTO> GetList()
        {
            var list = await _unitOfWork.iotLogRepository.GetAllAsync();
            if (list == null || list.Count == 0)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            var result = _mapper.Map<List<IOTLogView>>(list);
            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
        }

        // ==================== ĐÈN LED ====================

        /// <summary>
        /// Bật/tắt đèn LED (V12) - Chỉ có hiệu lực khi ở chế độ Manual (V7 = 1)
        /// </summary>
        public async Task<bool> ControlLightAsync(bool isOn)
        {
            string value = isOn ? "1" : "0";
            return await SendCommandAsync("V12", value);
        }

        /// <summary>
        /// Cấu hình ngưỡng BẬT đèn khi trời tối (V13)
        /// </summary>
        public async Task<bool> SetLightOnThresholdAsync(int value)
        {
            if (value < 0 || value > 1023) return false;
            return await SendCommandAsync("V13", value.ToString());
        }

        /// <summary>
        /// Cấu hình ngưỡng TẮT đèn khi trời sáng (V14)
        /// </summary>
        public async Task<bool> SetLightOffThresholdAsync(int value)
        {
            if (value < 0 || value > 1023) return false;
            return await SendCommandAsync("V14", value.ToString());
        }
    }
}
