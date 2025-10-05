using Infrastructure.ViewModel.Response;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implement
{
    public class WeatherService : IWeatherServices
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "fbe7011d37ee38f0eaa77f13e1b4c9c4"; // Thay bằng API key của bạn

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherResponse> GetWeatherAsync(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetStringAsync(url);
            var data = JObject.Parse(response);

            // File: WeatherService.cs (trong phần ánh xạ dữ liệu)

            return new WeatherResponse
            {
                // Vị trí & Thời gian
                CityName = (string)data["name"],
                // Chuyển đổi Unix Timestamp sang DateTime
                TimeStamp = DateTimeOffset.FromUnixTimeSeconds((long)data["dt"]).DateTime,

                // Nhiệt độ
                TemperatureC = (double)data["main"]["temp"],
                FeelsLikeC = (double)data["main"]["feels_like"],
                TempMinC = (double)data["main"]["temp_min"],
                TempMaxC = (double)data["main"]["temp_max"],

                // Điều kiện chung (lấy phần tử đầu tiên trong mảng "weather")
                Summary = (string)data["weather"][0]["main"],
                Description = (string)data["weather"][0]["description"],
                // Tạo URL Icon thủ công
                IconUrl = $"http://openweathermap.org/img/wn/{(string)data["weather"][0]["icon"]}@2x.png",

                // Các thông số khác
                Humidity = (int)data["main"]["humidity"],
                PressureHpa = (int)data["main"]["pressure"],
                WindSpeedMps = (double)data["wind"]["speed"],

                // Lượng Mưa (Sử dụng ?. để kiểm tra xem trường "rain" và "1h" có tồn tại không)
                RainVolumeMm = (double?)data["rain"]?["1h"]
            };
        }
    }
}
