using System;
using System.Collections.Generic;

namespace Domain;

public partial class Report
{
    public long ReportId { get; set; }

    public int? ReportType { get; set; }

    public DateOnly? ReportDate { get; set; }

    public string? FilePath { get; set; }

    public long GeneratedBy { get; set; }

    public virtual Account GeneratedByNavigation { get; set; } = null!;
}
