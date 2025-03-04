using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class OrderDetail
{
    public long OrderDetailId { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public long OrderId { get; set; }

    public long ProductId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
