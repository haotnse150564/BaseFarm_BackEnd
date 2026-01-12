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
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long createdBy { get; set; }
    public long? updatedBy { get; set; }
    public long? scheduleId { get; set; }
    public virtual Schedule Schedule { get; set; } = null!;
    public virtual ICollection<Staff_FarmActivity> StaffFarmActivities { get; set; } = new List<Staff_FarmActivity>();
    public virtual ICollection<ScheduleLog>? FarmActivityLogsInSchedule { get; set; } = new List<ScheduleLog>();

}
