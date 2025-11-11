using Domain.Enum;
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
        public class ScheduleResponseView
        {
            public DateOnly? StartDate { get; set; }

            public DateOnly? EndDate { get; set; }

            public int? Quantity { get; set; }

            public Status? Status { get; set; }

            public bool PesticideUsed { get; set; }

            public DiseaseStatus? DiseaseStatus { get; set; }

            public DateOnly? PlantingDate { get; set; }

            public DateOnly? HarvestDate { get; set; }

            public DateOnly? CreatedAt { get; set; }

            public DateOnly? UpdatedAt { get; set; }

            public string? ManagerName { get; set; }
            public string? StaffName { get; set; }
            public ViewAccount? Staff { get; set; }
            public FarmActivityView? farmActivityView { get; set; }
            public FarmView? farmView { get; set; }
            public CropView? cropView { get; set; }

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
