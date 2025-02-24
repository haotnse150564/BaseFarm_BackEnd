using System;
using System.Collections.Generic;

namespace Domain;

public partial class Order
{
    public long OrderId { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? Status { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public long CustomerId { get; set; }

    public virtual Account Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
