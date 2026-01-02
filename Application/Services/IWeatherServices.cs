using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IWeatherServices
    {
        Task<WeatherResponse> GetWeatherAsync(string city);
        Task<WeatherForecastResponse> GetForecastAsync(string city);
        Task<List<HourlyForecastResponse>> GetHourlyForecastAsync(string city, int maxHoursAhead = 6, long? managerId = null);
    }
}
