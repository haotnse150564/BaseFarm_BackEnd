using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Crop
{
    public long CropId { get; set; }

    public string? CropName { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? Origin { get; set; }

    public Status? Status { get; set; }

    public long CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual CropRequirement? CropRequirement { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
