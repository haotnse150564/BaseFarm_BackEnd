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

        public async Task<WeatherForecastResponse> GetForecastAsync(string city)
        {
            string encodedCity = Uri.EscapeDataString(city);
            string url = $"https://api.openweathermap.org/data/2.5/forecast" +
                         $"?q={encodedCity}" +
                         $"&appid={_apiKey}" +
                         $"&units=metric" +
                         $"&lang=vi";  // Tiếng Việt

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi API dự báo: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(json);

            // Kiểm tra thành phố có tìm thấy không
            if (data["cod"].ToString() != "200")
                throw new Exception($"Không tìm thấy thành phố: {city}");

            string cityName = $"{data["city"]["name"]}, {data["city"]["country"]}";

            var result = new WeatherForecastResponse
            {
                CityName = cityName,
                Latitude = (double)data["city"]["coord"]["lat"],
                Longitude = (double)data["city"]["coord"]["lon"],
                Timezone = "Asia/Bangkok" // hoặc lấy từ dữ liệu nếu cần
            };

            // Thời tiết hiện tại: lấy điểm đầu tiên (gần nhất)
            var first = data["list"][0];
            result.Current = new WeatherResponse
            {
                CityName = cityName,
                TimeStamp = DateTimeOffset.FromUnixTimeSeconds((long)first["dt"]).DateTime,
                TemperatureC = (double)first["main"]["temp"],
                FeelsLikeC = (double)first["main"]["feels_like"],
                TempMinC = (double)first["main"]["temp_min"],
                TempMaxC = (double)first["main"]["temp_max"],
                Humidity = (int)first["main"]["humidity"],
                PressureHpa = (int)first["main"]["pressure"],
                WindSpeedMps = (double)first["wind"]["speed"],
                Summary = (string)first["weather"][0]["main"],
                Description = (string)first["weather"][0]["description"],
                IconUrl = $"http://openweathermap.org/img/wn/{(string)first["weather"][0]["icon"]}@2x.png",
                RainVolumeMm = first["rain"]?["3h"] != null ? (double?)first["rain"]["3h"] / 3 : null // ước lượng mm/giờ
            };

            // Dự báo hàng ngày: nhóm theo ngày và lấy giá trị đại diện (thường là 12h trưa hoặc cao nhất)
            var dailyGroups = data["list"]
                .GroupBy(item => DateTimeOffset.FromUnixTimeSeconds((long)item["dt"]).Date);

            foreach (var group in dailyGroups.Take(7)) // chỉ lấy 7 ngày
            {
                // Lấy điểm gần 12h trưa nhất để làm đại diện ngày
                var representative = group
                    .OrderBy(x => Math.Abs(((DateTimeOffset.FromUnixTimeSeconds((long)x["dt"])).Hour - 12)))
                    .First();

                result.Daily.Add(new DailyForecast
                {
                    Date = DateTimeOffset.FromUnixTimeSeconds((long)representative["dt"]).DateTime,
                    TempMaxC = (double)representative["main"]["temp_max"],
                    TempMinC = (double)representative["main"]["temp_min"],
                    FeelsLikeDayC = (double)representative["main"]["feels_like"],
                    Humidity = (int)representative["main"]["humidity"],
                    WindSpeedMps = (double)representative["wind"]["speed"],
                    Pop = group.Sum(x => (double?)(x["rain"]?["3h"] ?? 0) ?? 0) > 0 ? 70 : 0, // đơn giản hóa % mưa
                    Summary = (string)representative["weather"][0]["main"],
                    Description = (string)representative["weather"][0]["description"],
                    IconUrl = $"http://openweathermap.org/img/wn/{(string)representative["weather"][0]["icon"]}@2x.png"
                });
            }

            return result;
        }

        /// <summary>
        /// Lấy dự báo thời tiết theo giờ từ hiện tại đến hours giờ sau (tối đa 3-9 giờ tùy dữ liệu)
        /// API trả về mỗi 3 giờ 1 lần → sẽ lấy các điểm gần nhất
        /// </summary>
        public async Task<List<HourlyForecastResponse>> GetHourlyForecastAsync(string city, int maxHoursAhead = 6)
        {
            string encodedCity = Uri.EscapeDataString(city);
            string url = $"https://api.openweathermap.org/data/2.5/forecast" +
                         $"?q={encodedCity}" +
                         $"&appid={_apiKey}" +
                         $"&units=metric" +
                         $"&lang=vi";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi API dự báo theo giờ: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(json);

            if (data["cod"].ToString() != "200")
                throw new Exception($"Không tìm thấy thành phố: {city}");

            string cityName = $"{data["city"]["name"]}, {data["city"]["country"]}";

            var forecastList = new List<HourlyForecastResponse>();

            DateTime now = DateTime.UtcNow; // Thời gian hiện tại theo UTC (API dùng UTC)

            foreach (var item in data["list"])
            {
                DateTime forecastTimeUtc = DateTimeOffset.FromUnixTimeSeconds((long)item["dt"]).DateTime;

                // Chuyển về giờ Việt Nam để dễ hiểu hơn (UTC+7)
                DateTime forecastTimeVn = forecastTimeUtc.AddHours(7);

                TimeSpan diff = forecastTimeVn - DateTime.Now; // So sánh với giờ máy chủ/local (gần đúng giờ VN)

                // Chỉ lấy các mốc từ hiện tại trở đi
                if (diff.TotalHours >= -1 && diff.TotalHours <= maxHoursAhead + 1)
                {
                    string label;

                    if (diff.TotalMinutes <= 60) // trong vòng 1 giờ
                    {
                        if (diff.TotalMinutes < 5)
                            label = "Bây giờ";
                        else
                        {
                            int minutes = (int)Math.Round(diff.TotalMinutes);
                            label = $"{minutes} phút nữa";
                        }
                    }
                    else // trên 1 giờ
                    {
                        int hours = (int)Math.Floor(diff.TotalHours);
                        int minutes = (int)Math.Round(diff.TotalMinutes - hours * 60);

                        if (minutes == 0)
                            label = hours == 1 ? "1 giờ nữa" : $"{hours} giờ nữa";
                        else
                            label = $"{hours} giờ {minutes} phút nữa";
                    }

                    var weather = new HourlyForecastResponse
                    {
                        CityName = cityName,
                        TimeStamp = forecastTimeVn, // Hiển thị giờ Việt Nam luôn cho chính xác
                        ForecastFor = label,
                        TemperatureC = (double)item["main"]["temp"],
                        FeelsLikeC = (double)item["main"]["feels_like"],
                        Humidity = (int)item["main"]["humidity"],
                        WindSpeedMps = (double)item["wind"]["speed"],
                        Summary = (string)item["weather"][0]["main"],
                        Description = (string)item["weather"][0]["description"],
                        IconUrl = $"http://openweathermap.org/img/wn/{(string)item["weather"][0]["icon"]}@2x.png",
                        RainVolumeMm = item["rain"]?["3h"] != null ? (double?)item["rain"]["3h"] : null
                    };

                    forecastList.Add(weather);
                }
            }

            if (forecastList.Count == 0)
                throw new Exception("Không có dữ liệu dự báo trong khoảng thời gian gần.");

            return forecastList;
        }
    }
}
