using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain;

public partial class Account
{
    public long AccountId { get; set; }

    public int? Phone { get; set; }

    public string? PasswordHash { get; set; }

    public Roles Role { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public int? Status { get; set; }

    public virtual AccountProfile? AccountProfile { get; set; }

    public virtual ICollection<FarmActivity> FarmActivities { get; set; } = new List<FarmActivity>();

    public virtual ICollection<FarmDetail> FarmDetails { get; set; } = new List<FarmDetail>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
