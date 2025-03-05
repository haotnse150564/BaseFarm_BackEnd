using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class FarmActivity
{
    public int FarmActivitiesId { get; set; }

    public int? ActivityType { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public Status? Status { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
