using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Crop
{
    public long CropId { get; set; }

    public string? CropName { get; set; }

    public string? Description { get; set; }

    public int? Quantity { get; set; }

    public Status? Status { get; set; }

    public DateOnly? PlantingDate { get; set; }

    public DateOnly? HarvestDate { get; set; }

    public virtual CropRequirement? CropRequirement { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
