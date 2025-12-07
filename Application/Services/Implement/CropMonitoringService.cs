using Application;
using Application.Services;
using Application.Services.Implement;
using Application.Utils;
using Domain.Enum;
using Domain.Model;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;
using static Application.Services.Implement.NotificationHub;

public class CropMonitoringService : ICropMonitoringService
{
    private readonly IWeatherServices _weatherService;
    private readonly IUnitOfWorks _unitOfWork;
    private readonly IHubContext<ManagerNotificationHub> _hubContext;
    private readonly JWTUtils _jwt ;

    public CropMonitoringService(
        IWeatherServices weatherService,
        IUnitOfWorks unitOfWork,
        IHubContext<ManagerNotificationHub> hubContext,
        JWTUtils jwtUtils)
    {
        _weatherService = weatherService;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
        _jwt = jwtUtils;
    }

    /// <summary>
    /// Kiểm tra nhiều crop cùng lúc theo thành phố
    /// </summary>
    public async Task CheckWeatherAndNotifyAllCropsAsync(string city)
    {
        var getCurrentUserId = await _jwt.GetCurrentUserAsync();
        if (getCurrentUserId == null) return;
        if (getCurrentUserId.Role != Roles.Manager) return;
        // Lấy danh sách tất cả crop requirement từ DB
        var cropRequirements = await _unitOfWork.cropRequirementRepository.GetAllAsync();
        if (cropRequirements == null || !cropRequirements.Any()) return;

        // Lấy dữ liệu thời tiết một lần
        var weather = await _weatherService.GetWeatherAsync(city);

        foreach (var cropRequirement in cropRequirements)
        {
            var alerts = new List<string>();

            // So sánh nhiệt độ (±10%)
            if (cropRequirement.Temperature.HasValue)
            {
                var threshold = cropRequirement.Temperature.Value;
                var actual = Convert.ToDecimal(weather.TemperatureC);

                if (actual > threshold * 1.1m || actual < threshold * 0.9m)
                {
                    alerts.Add($"Nhiệt độ {weather.TemperatureC:F1}°C lệch quá 10% so với ngưỡng {threshold}°C cho crop {cropRequirement.CropId}");
                }
            }

            // So sánh độ ẩm (±10%)
            if (cropRequirement.Moisture.HasValue)
            {
                var threshold = cropRequirement.Moisture.Value;
                var actual = weather.Humidity;

                if (actual > threshold * 1.1m || actual < threshold * 0.9m)
                {
                    alerts.Add($"Độ ẩm {actual}% lệch quá 10% so với ngưỡng {threshold}% cho crop {cropRequirement.CropId}");
                }
            }

            // Kiểm tra ánh sáng (ví dụ: nếu trời nhiều mây thì coi như không đủ ánh sáng)
            if (cropRequirement.LightRequirement.HasValue && weather.Summary.Contains("Cloud"))
            {
                alerts.Add($"Ánh sáng không đủ cho crop {cropRequirement.CropId} (yêu cầu {cropRequirement.LightRequirement} lux)");
            }

            // Kiểm tra tần suất tưới (ví dụ: nếu mưa nhiều hơn 10% so với mức tưới)

            //if (cropRequirement.WateringFrequency.HasValue && weather.RainVolumeMm.HasValue)
            //{
            //    var threshold = cropRequirement.WateringFrequency.Value; // decimal
            //    var actual = Convert.ToDecimal(weather.RainVolumeMm.Value); // ép double sang decimal

            //    if (actual > threshold * 1.1m || actual < threshold * 0.9m)
            //    {
            //        alerts.Add($"Lượng mưa {actual}mm lệch quá 10% so với tần suất tưới {threshold} lần/ngày cho crop {cropRequirement.CropId}");
            //    }

            //}
            // Nếu có cảnh báo thì gửi cho Manager
            if (alerts.Any())
            {
                var notification = new WeatherAlertNotification
                {
                    CropId = cropRequirement.CropId,
                    TimeStamp = weather.TimeStamp,
                    City = city,
                    Alerts = alerts
                };

                await _hubContext.Clients
                    .Group($"User_{2}")
                    .SendAsync("ReceiveWeatherAlert", notification);
            }
        }
    }



internal class WeatherAlertNotification
    {
        public long CropId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string City { get; set; }
        public List<string> Alerts { get; set; }
    }
}