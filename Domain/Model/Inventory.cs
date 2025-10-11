using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Inventory
{
    public long InventoryId { get; set; }
    public string? ItemType { get; set; }
    public string? Location { get; set; }
    public string? Unit { get; set; } = "Bó";

    public int? StockQuantity { get; set; }
    public Status Status { get; set; }
    public DateOnly ExpireDate { get; set; }
    public DateOnly CreateAt { get; set; }
    public long ProductId { get; set; }
    public long ScheduleId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Schedule Schedule { get; set; } = null!;
}
