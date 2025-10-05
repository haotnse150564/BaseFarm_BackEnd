using Application.Services;
using Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherServices _weatherService;

        public WeatherController(IWeatherServices weatherService)
        {
            _weatherService = weatherService;
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
    }
}

