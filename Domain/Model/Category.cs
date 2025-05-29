using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Category
{
    public long CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<Crop>? Crops { get; set; } = new List<Crop>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
