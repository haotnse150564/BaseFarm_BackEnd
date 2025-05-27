using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Feedback
{
    public long FeedbackId { get; set; }

    public string? Comment { get; set; }

    public int? Rating { get; set; }

    public Status? Status { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public long CustomerId { get; set; }

    public long OrderDetailId { get; set; }

    public virtual Account Customer { get; set; } = null!;

    public virtual OrderDetail OrderDetail { get; set; } = null!;
}
