﻿using Domain.Enum;
using System;
using System.Collections.Generic;

namespace Domain.Model;

public partial class Schedule
{
    public long ScheduleId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? Quantity { get; set; }

    public Status? Status { get; set; }

    public bool PesticideUsed { get; set; }

    public DiseaseStatus? DiseaseStatus { get; set; }

    public DateOnly? PlantingDate { get; set; }

    public DateOnly? HarvestDate { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public long AssignedTo { get; set; }

    public long FarmId { get; set; }

    public long CropId { get; set; }

    public virtual Account AssignedToNavigation { get; set; } = null!;

    public virtual Crop Crop { get; set; } = null!;

    public virtual FarmActivity? FarmActivities { get; set; }

    public virtual Farm FarmDetails { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
