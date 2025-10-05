using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class WeatherResponse
    {
        //public string City { get; set; }
        //public double Temperature { get; set; }
        //public string Description { get; set; }
        //public int Humidity { get; set; }


        public string CityName { get; set; }
        public DateTime TimeStamp { get; set; }

        // Nhiệt độ
        public double TemperatureC { get; set; }
        public double FeelsLikeC { get; set; }
        public double TempMinC { get; set; }
        public double TempMaxC { get; set; }

        // Điều kiện chung
        public string Summary { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; } // URL để hiển thị icon

        // Các thông số khác
        public int Humidity { get; set; } // %
        public double WindSpeedMps { get; set; } // m/s
        public int PressureHpa { get; set; } // hPa
        public double? RainVolumeMm { get; set; } // mm (trong 1 giờ)
    }
}
