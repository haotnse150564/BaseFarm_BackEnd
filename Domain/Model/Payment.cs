using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Payment
{
    public long PaymentId { get; set; }
    public long OrderId { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string VnPayResponseCode { get; set; }
    public bool Success { get; set; }
    public DateTime PaymentTime { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public virtual Order Order { get; set; } = null!;
}
