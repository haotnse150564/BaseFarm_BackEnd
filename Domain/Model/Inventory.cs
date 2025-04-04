﻿using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Inventory
{
    public long InventoryId { get; set; }

    public string? Location { get; set; }

    public int? StockQuantity { get; set; }

    public Status? Status { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public long ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
