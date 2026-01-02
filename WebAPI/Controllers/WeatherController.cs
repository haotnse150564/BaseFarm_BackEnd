using Application.Services;
using Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

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

        [HttpGet("forecast")]
        public async Task<IActionResult> GetForecast([FromQuery] string city = "Hanoi")
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("Vui lòng nhập tên thành phố.");

            try
            {
                var forecast = await _weatherService.GetForecastAsync(city);
                return Ok(forecast);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("hourly")]
        [Authorize(Roles = "Manager")] // Chỉ Manager mới được gọi
        public async Task<IActionResult> GetHourlyForecast(
        [FromQuery] string city = "Ho Chi Minh",
        [FromQuery] int hours = 24)
        {            
            try
            {
                var forecast = await _weatherService.GetHourlyForecastAsync(city, hours);

                return Ok(new
                {
                    city,
                    note = "Dự báo được cập nhật mỗi 3 giờ một lần.",
                    requested_hours = hours,
                    current_time_vn = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                    rain_alert_sent = forecast.Any(f => f.RainVolumeMm > 0.3),
                    count = forecast.Count,
                    data = forecast
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}


