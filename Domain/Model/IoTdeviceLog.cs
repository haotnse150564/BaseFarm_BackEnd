using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class IoTdeviceLog
{
    public long LogId { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public long IoTdevice { get; set; }

    public long TrackingId { get; set; }

    public virtual IoTdevice IoTdeviceNavigation { get; set; } = null!;

    public virtual DailyLog Tracking { get; set; } = null!;
}
