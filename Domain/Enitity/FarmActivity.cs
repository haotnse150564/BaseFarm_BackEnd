using System;
using System.Collections.Generic;

namespace Domain;

public partial class FarmActivity
{
    public long FarmActivitiesId { get; set; }

    public int? ActivityType { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? Status { get; set; }

    public long AssignedTo { get; set; }

    public long FarmId { get; set; }

    public virtual Account AssignedToNavigation { get; set; } = null!;

    public virtual FarmDetail Farm { get; set; } = null!;
}
