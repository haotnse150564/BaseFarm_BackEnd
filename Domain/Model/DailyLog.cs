using Domain.Enum;
using Domain.Model;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class DailyLog
{
    public long TrackingId { get; set; }

    public DateOnly? Date { get; set; }

    public string? Notes { get; set; }

    public Status? Status { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public long AssignedTo { get; set; }

    public long ScheduleId { get; set; }

    public virtual Account AssignedToNavigation { get; set; } = null!;

    public virtual ICollection<IoTdeviceLog> IoTdeviceLogs { get; set; } = new List<IoTdeviceLog>();

    public virtual Schedule Schedule { get; set; } = null!;
}
