﻿using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Account
{
    public long AccountId { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public Roles? Role { get; set; }

    public Status? Status { get; set; }

    public string? RefreshToken { get; set; }

    public int? ExpireMinute { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public virtual AccountProfile? AccountProfile { get; set; }

    public virtual ICollection<DailyLog> DailyLogs { get; set; } = new List<DailyLog>();

    public virtual ICollection<Farm> Farms { get; set; } = new List<Farm>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
