using System;
using System.Collections.Generic;

namespace Domain;

public partial class IoTdevice
{
    public long IoTdevicesId { get; set; }

    public string? DeviceType { get; set; }

    public int? Status { get; set; }

    public string? SensorValue { get; set; }

    public string? Unit { get; set; }

    public int? DeviceName { get; set; }

    public DateOnly? LastUpdate { get; set; }

    public long FarmId { get; set; }

    public virtual FarmDetail Farm { get; set; } = null!;
}
