using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class CropRequirement
{
    public long RequirementId { get; set; }

    public int? EstimatedDate { get; set; }

    public decimal? Moisture { get; set; }

    public decimal? Temperature { get; set; }

    public string? Fertilizer { get; set; }

    public long DeviceId { get; set; }

    public virtual IoTdevice Device { get; set; } = null!;

    public virtual Crop Requirement { get; set; } = null!;
}
