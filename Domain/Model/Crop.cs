using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Crop
{
    public long CropId { get; set; }

    public string? CropName { get; set; }

    public string? Description { get; set; }

    public Status? Status { get; set; }

    public DateOnly? PlantingDate { get; set; }

    public DateOnly? HarvestDate { get; set; }

    public decimal? Moisture { get; set; }

    public decimal? Temperature { get; set; }

    public string? Fertilizer { get; set; }

    public long FarmDetailsId { get; set; }

    public virtual FarmDetail FarmDetails { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
