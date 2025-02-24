using System;
using System.Collections.Generic;

namespace Domain;

public partial class FarmDetail
{
    public long FarmDetailsId { get; set; }

    public string? FarmName { get; set; }

    public string? Location { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public long StaffId { get; set; }

    public virtual ICollection<FarmActivity> FarmActivities { get; set; } = new List<FarmActivity>();

    public virtual ICollection<IoTdevice> IoTdevices { get; set; } = new List<IoTdevice>();

    public virtual Account Staff { get; set; } = null!;
}
