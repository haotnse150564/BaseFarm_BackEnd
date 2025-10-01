using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class CropRequirement
{
    public long CropRequirementId { get; set; }
    public PlantStage? PlantStage { get; set; }

    public int? EstimatedDate { get; set; }

    public decimal? Moisture { get; set; }

    public decimal? Temperature { get; set; }

    public string? Fertilizer { get; set; }


    public virtual Crop? Crop { get; set; } = null!;
}
