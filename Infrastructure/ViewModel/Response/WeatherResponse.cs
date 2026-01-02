using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class WeatherResponse
    {

        public string CityName { get; set; }
        public DateTime TimeStamp { get; set; }

        // Nhiệt độ
        public double TemperatureC { get; set; }
        public double FeelsLikeC { get; set; } //Nhiệt độ cảm nhận được
        public double TempMinC { get; set; }
        public double TempMaxC { get; set; }

        // Điều kiện chung
        public string Summary { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; } // URL để hiển thị icon

        // Các thông số khác
        public int Humidity { get; set; } // % Độ ẩm tương đối
        public double WindSpeedMps { get; set; } // m/s 
        public int PressureHpa { get; set; } // hPa Áp suất khí quyển 
        public double? RainVolumeMm { get; set; } // mm (trong 1 giờ)
    }

    public class WeatherForecastResponse
    {
        public string CityName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; } = string.Empty;

        // Thời tiết hiện tại
        public WeatherResponse Current { get; set; } = new WeatherResponse();

        // Dự báo 7-8 ngày tới
        public List<DailyForecast> Daily { get; set; } = new List<DailyForecast>();
    }

    public class DailyForecast
    {
        public DateTime Date { get; set; }                   // Ngày dự báo (ví dụ: ngày mai, ngày kia...)
        public double TempMaxC { get; set; }
        public double TempMinC { get; set; }
        public double FeelsLikeDayC { get; set; }
        public int Humidity { get; set; }
        public double WindSpeedMps { get; set; }
        public double Pop { get; set; }                      // % khả năng mưa (0-100)
        public string Summary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // bằng tiếng Việt
        public string IconUrl { get; set; } = string.Empty;
    }

    public class HourlyForecastResponse
    {
        public string CityName { get; set; } = string.Empty;
        // Thời gian thực tế của dữ liệu dự báo
        public DateTime TimeStamp { get; set; }
        public string ForecastFor { get; set; } = string.Empty; // ví dụ: "Bây giờ", "3 giờ nữa", "6 giờ nữa"
        // Các thông tin thời tiết
        public double TemperatureC { get; set; }
        public double FeelsLikeC { get; set; }
        public int Humidity { get; set; }
        public double WindSpeedMps { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public double? RainVolumeMm { get; set; }
    }
}
