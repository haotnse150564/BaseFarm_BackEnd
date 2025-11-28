using AutoMapper;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.IOTLogResponse;

namespace Application.Services.Implement
{
    public class IOTLogServices : IIOTLogServices
    {
        private readonly HttpClient _httpClient;
        private const string BlynkToken = "xRd0sDuPYqFjPI1ZSHRr7Bd1cJq3fH2Y";
        private const string BlynkBaseUrl = "https://sgp1.blynk.cloud";
        private const string BlynkWriteBaseUrl = "https://sgp1.blynk.cloud/external/api";
        private readonly IUnitOfWorks _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBlynkService _devicesServices;
        public IOTLogServices(HttpClient httpClient, IUnitOfWorks unitOfWorks, IMapper mapper, IBlynkService services)
        {
            _httpClient = httpClient;
            _unitOfWork = unitOfWorks;
            _mapper = mapper;
            _devicesServices = services;
        }



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
                                   .OrderBy(kvp => kvp.Key)   // parse key thành DateTime
                                   .ToList();

                string[] DeviceName = ["Nhiet Do", "Do Am", "Luong Mua", "Do Am Dat", "Anh Sang"];
                List<LogEntry> logEntries = new List<LogEntry>();
                for(int i = 0; i < 5; i++)
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
                    await _unitOfWork.iotLogRepository.AddAsync(iotLog);
                    await _unitOfWork.SaveChangesAsync();
                    // Lưu iotLog vào cơ sở dữ liệu của bạn ở đây
                    // Ví dụ: _dbContext.IOTLogs.Add(iotLog);
                }
                return "Log đã được cập nhật";
            }
            catch (Exception ex)
            {
                return "Có lỗi xảy ra";
            }

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
        class LogEntry
        {
            public string Pin { get; set; }
            public double Value { get; set; }
            public string DeviceName { get; set; }
        }
        public async Task<ResponseDTO> GetList()
        {
            var list = await _unitOfWork.iotLogRepository.GetAllAsync();
            if(list == null || list.Count == 0)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            var result = _mapper.Map<List<IOTLogView>>(list);
            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
        }
    }
}
