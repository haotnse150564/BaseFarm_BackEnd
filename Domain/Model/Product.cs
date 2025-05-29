using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Product
{
    public long ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? Images { get; set; }

    public decimal? Price { get; set; }

    public int? StockQuantity { get; set; }

    public string? Description { get; set; }

    public ProductStatus? Status { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public long CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Crop ProductNavigation { get; set; } = null!;
}
