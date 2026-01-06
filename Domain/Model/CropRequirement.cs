using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class CropRequirement
{
    public long CropRequirementId { get; set; }
    public long CropId { get; set; }
    public PlantStage? PlantStage { get; set; }

    public int? EstimatedDate { get; set; }
    public decimal? SoilMoisture { get; set; }
    public decimal? Temperature { get; set; }
    public string? Fertilizer { get; set; }
    public decimal? LightRequirement { get; set; }
    public decimal? Humidity { get; set; }
    public int? WateringFrequency { get; set; }
    public string? Notes { get; set; }

    public DateOnly CreatedDate { get; set; }
    public DateOnly? UpdatedDate { get; set; }
    public bool IsActive { get; set; } = true;


    public virtual Crop? Crop { get; set; } = null!;
}
