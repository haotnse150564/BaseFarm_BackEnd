using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Farm
{
    public long FarmId { get; set; }

    public string? FarmName { get; set; }

    public string? Location { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public long AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;


    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<FarmEquipment>? FarmEquipments { get; set; } = new List<FarmEquipment>();
}
