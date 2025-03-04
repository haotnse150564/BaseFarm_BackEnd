using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Schedule
{
    public long ScheduleId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? Status { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public long AssignedTo { get; set; }

    public int FarmActivityId { get; set; }

    public long FarmDetailsId { get; set; }

    public virtual Account AssignedToNavigation { get; set; } = null!;

    public virtual FarmActivity FarmActivity { get; set; } = null!;

    public virtual FarmDetail FarmDetails { get; set; } = null!;
}
