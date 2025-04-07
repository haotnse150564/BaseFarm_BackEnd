using Domain.Enum;
using Domain.Model;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Schedule
{
    public long ScheduleId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public Status? Status { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public long AssignedTo { get; set; }

    public int FarmActivityId { get; set; }

    public long FarmDetailsId { get; set; }

    public long CropId { get; set; }

    public virtual Account AssignedToNavigation { get; set; } = null!;

    public virtual Crop Crop { get; set; } = null!;

    public virtual ICollection<DailyLog> DailyLogs { get; set; } = new List<DailyLog>();

    public virtual FarmActivity FarmActivity { get; set; } = null!;

    public virtual Farm FarmDetails { get; set; } = null!;
}
