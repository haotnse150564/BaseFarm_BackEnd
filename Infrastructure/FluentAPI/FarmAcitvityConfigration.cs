﻿using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FluentAPI
{
    public class FarmAcitvityConfigration : IEntityTypeConfiguration<FarmActivity>
    {
        public void Configure(EntityTypeBuilder<FarmActivity> builder)
        {

            builder.HasKey(e => e.FarmActivitiesId);

            builder.ToTable("FarmActivity");

            builder.Property(e => e.ActivityType).HasColumnName("activityType");
            builder.Property(e => e.EndDate).HasColumnName("endDate");
            builder.Property(e => e.ScheduleId).HasColumnName("scheduleId");
            builder.Property(e => e.StartDate).HasColumnName("startDate");
            builder.Property(e => e.Status).HasColumnName("status");

            builder.HasOne(d => d.Schedule).WithMany(p => p.FarmActivities)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKFarmActivi709275");
        }
    }
}
