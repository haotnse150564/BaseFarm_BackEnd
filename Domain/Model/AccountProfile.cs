﻿using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class AccountProfile
{
    public long AccountProfileId { get; set; }

    public Gender? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Fullname { get; set; }

    public string? Address { get; set; }

    public string? Images { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public virtual Account AccountProfileNavigation { get; set; } = null!;
}
