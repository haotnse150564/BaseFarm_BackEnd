using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Device
{
    public long DevicesId { get; set; }

    public string? DeviceName { get; set; }

    public string? DeviceType { get; set; }

    public Status? Status { get; set; }

    public DateOnly? LastUpdate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public long FarmDetailsId { get; set; }

    // public virtual ICollection<CropRequirement> CropRequirements { get; set; } = new List<CropRequirement>();
    public virtual ICollection<FarmEquipment> FarmEquipments { get; set; } = new List<FarmEquipment>();
    public virtual ICollection<IOTLog> Log { get; set; } = new List<IOTLog>();

    public virtual Farm FarmDetails { get; set; } = null!;
}
