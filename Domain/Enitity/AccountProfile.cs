using System;
using System.Collections.Generic;

namespace Domain;

public partial class AccountProfile
{
    public long AccountProfileId { get; set; }

    public string? Email { get; set; }

    public int? Gender { get; set; }

    public string? Images { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public virtual Account AccountProfileNavigation { get; set; } = null!;
}
