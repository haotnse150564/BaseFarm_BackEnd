using Application.Services;
using Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherServices _weatherService;
        private readonly ICropMonitoringService _monitoringService;

        public WeatherController(IWeatherServices weatherService, ICropMonitoringService cropMonitoringService)
        {
            _weatherService = weatherService;
            _monitoringService = cropMonitoringService;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> Get(string city)
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherAsync(city);
                return Ok(weatherData); // Trả về dữ liệu thời tiết
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> CheckWeather(string city = "Ho Chi Minh")
        {
            await _monitoringService.CheckWeatherAndNotifyAllCropsAsync(city);
            return Ok("Đã kiểm tra và gửi cảnh báo nếu vượt ngưỡng.");
        }
    }
}


