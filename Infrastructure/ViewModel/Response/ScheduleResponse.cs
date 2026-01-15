using Domain.Enum;
using Domain.Model;
using System.Text.Json.Serialization;
using static Infrastructure.ViewModel.Response.AccountResponse;
using static Infrastructure.ViewModel.Response.CropRequirementResponse;
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
            public long ScheduleId { get; set; }
            public long CropId { get; set; }
            public long ManagerId { get; set; }
            public DateOnly? StartDate { get; set; }

            public DateOnly? EndDate { get; set; }
            public string? CurrentPlantStage { get; set; }
            public int? Quantity { get; set; }
            public string? HarvestedQuantity { get; set; }
            public Status? Status { get; set; }

            public bool PesticideUsed { get; set; }

            public DiseaseStatus? DiseaseStatus { get; set; }

            //public DateOnly? PlantingDate { get; set; }

          //  public DateOnly? HarvestDate { get; set; }

            public DateOnly? CreatedAt { get; set; }

            public DateOnly? UpdatedAt { get; set; }

            public ViewAccount? Manager { get; set; }
            public List<FarmActivityView>? farmActivityView { get; set; }
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
        public class ScheduleLogResponse
        {
            public long CropLogId { get; set; }
            public long FarmActivityId { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
            public string CreateBy { get; set; }
            public DateTime UpdatedAt { get; set; }
            public string UpdateBy { get; set; }
        }
        public class UpdateTodayResponse
        {
            public DateOnly Today { get; set; }
            public PlantStage CurrentStage { get; set; }
            public int DaysSinceStart { get; set; }

            // Các yêu cầu chăm sóc của giai đoạn hiện tại
            public CropRequirementDto? CurrentStageRequirements { get; set; }

            public CropRequirementDto? NextStageRequirements { get; set; }
            public int? DaysToNextStage { get; set; }
        }

        
    }
}
