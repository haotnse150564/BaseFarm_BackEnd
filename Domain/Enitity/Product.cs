﻿using System;
using System.Collections.Generic;

namespace Domain;

public partial class Product
{
    public long ProductId { get; set; }

    public int? ProductName { get; set; }

    public decimal? Price { get; set; }

    public int? StockQuantity { get; set; }

    public string? Description { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public long CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
