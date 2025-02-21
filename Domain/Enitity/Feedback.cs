﻿using System;
using System.Collections.Generic;

namespace Domain;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string? Comment { get; set; }

    public int? Rating { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public long CustomerId { get; set; }

    public virtual Account Customer { get; set; } = null!;
}
