using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class FarmActivity //update lên bảng cần sau này
{
    public long FarmActivitiesId { get; set; }

    public ActivityType? ActivityType { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public FarmActivityStatus? Status { get; set; }

   // public long? ScheduleId { get; set; }

    public virtual ICollection <Schedule> Schedule { get; set; } = null!;
}
