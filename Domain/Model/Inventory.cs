using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Inventory
{
    public long InventoryId { get; set; }

    public string? Location { get; set; }

    public int? StockQuantity { get; set; }

    public int? Status { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public long ProductId { get; set; }

    public long ScheduleId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Schedule Schedule { get; set; } = null!;
}
