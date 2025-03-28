using System.Text.Json.Serialization;
using static Infrastructure.ViewModel.Response.AccountProfileResponse;
using static Infrastructure.ViewModel.Response.AccountResponse;

namespace Infrastructure.ViewModel.Response
{
    public class ScheduleResponse
    {
        public class ViewSchedule
        {
            [JsonIgnore]
            public long ScheduleId { get; set; }
            [JsonIgnore]
            public long AssignedTo { get; set; }
            public string? FullNameStaff { get; set; }

            public DateOnly? StartDate { get; set; }

            public DateOnly? EndDate { get; set; }

            public string? Status { get; set; }

            public DateOnly? CreatedAt { get; set; }

            public DateOnly? UpdatedAt { get; set; }
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
