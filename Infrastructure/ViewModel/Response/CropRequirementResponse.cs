using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class CropRequirementResponse
    {

        public class CropRequirementView()
        {
            public long CropRequirementId { get; set; }
            public long CropId { get; set; }
            public string? PlantStage { get; set; }
            public int? EstimatedDate { get; set; }
            public decimal? SoilMoisture { get; set; }
            public decimal? Humidity { get; set; }
            public decimal? Temperature { get; set; }
            public string? Fertilizer { get; set; }
            public decimal? LightRequirement { get; set; }
            public string? WateringFrequency { get; set; }
            public string? Notes { get; set; }
            public bool IsActive { get; set; }
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

        public class CropRequirementDto
        {
            public PlantStage PlantStage { get; set; }
            public int EstimatedDate { get; set; } // ngày tích lũy kết thúc stage
            public decimal? SoilMoisture { get; set; }
            public decimal? Humidity { get; set; }
            public decimal? Temperature { get; set; }
            public string? Fertilizer { get; set; }
            public decimal? LightRequirement { get; set; }
            public int? WateringFrequency { get; set; }
            public string? Notes { get; set; }
        }
    }
}
