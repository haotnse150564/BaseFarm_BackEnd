using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class IoTdevice
{
    public long IoTdevicesId { get; set; }

    public string? DeviceName { get; set; }

    public string? DeviceType { get; set; }

    public Status? Status { get; set; }

    public string? SensorValue { get; set; }

    public string? Unit { get; set; }

    public DateOnly? LastUpdate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public long FarmDetailsId { get; set; }

    public virtual FarmDetail FarmDetails { get; set; } = null!;
}
