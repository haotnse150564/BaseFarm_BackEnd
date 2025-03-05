
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Payment
{
    public long PaymentId { get; set; }

    public int? PaymentMethod { get; set; }

    public int? PaymentStatus { get; set; }

    public DateOnly? TransactionDate { get; set; }

    public long OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;
}
