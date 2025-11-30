using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Crop
{
    public long CropId { get; set; }

    public string? CropName { get; set; }

    public string? Description { get; set; }

    public string? Origin { get; set; }

    public string? Images { get; set; }
    public CropStatus? Status { get; set; }

    public DateOnly CreateAt { get; set; }

    public DateOnly? UpdateAt { get; set; }

    public long? CategoryId { get; set; }

    public virtual Category? Category { get; set; } = null!;

    public virtual ICollection<CropRequirement>? CropRequirement { get; set; }

    public virtual Product? Product { get; set; }
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
