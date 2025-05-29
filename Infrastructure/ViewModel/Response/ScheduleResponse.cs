using Domain.Model;
using System.Text.Json.Serialization;
using static Infrastructure.ViewModel.Response.AccountResponse;
using static Infrastructure.ViewModel.Response.CropResponse;
using static Infrastructure.ViewModel.Response.DailyLogResponse;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;
using static Infrastructure.ViewModel.Response.FarmDetailResponse;

namespace Infrastructure.ViewModel.Response
{
    public class ScheduleResponse
    {
        public class ViewSchedule
        {
            public long ScheduleId { get; set; }
            public string? FullNameStaff { get; set; }

            public string? StartDate { get; set; }

            public string? EndDate { get; set; }

            public string? Status { get; set; }

            public string? CreatedAt { get; set; }

            public string? PlantingDate { get; set; }
            public string? UpdatedAt { get; set; }
            public string? ActivityType { get; set; }
            public ViewAccount? accountView { get; set; }
            public IEnumerable <FarmActivityView>? farmActivityView { get; set; }
            public FarmView? farmDetailView { get; set; } 
            public CropView? cropView { get; set; }
            public List<DailyLogView>? dailyLog { get; set; }

            public long AssignedTo { get; set; }
            public List<int> FarmActivityId { get; set; }
            public long FarmId { get; set; }
            public long CropId { get; set; }
        }
        public class ResponseDTO
        {
            public int Status { get; set; }
            public string? Message { get; set; }
            public object? Data { get; set; }
            public ResponseDTO(int status, string? message, object? data = null)
            {
                Status = status;
                Message = message;
                Data = data;
            }
        }
    }
}
