using Domain.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FluentAPI
{
    public class ScheduleConfigration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable("Schedule");

            builder.HasIndex(e => e.AssignedTo, "IX_Schedule_assignedTo");

            builder.HasIndex(e => e.CropId, "IX_Schedule_cropID");

            builder.HasIndex(e => e.FarmId, "IX_Schedule_farmDetailsID");
            builder.HasKey(e => e.ScheduleId);
            builder.Property(e => e.AssignedTo).HasColumnName("assignedTo");
            builder.Property(e => e.CreatedAt).HasColumnName("createdAt");
            builder.Property(e => e.CropId).HasColumnName("cropID");
            builder.Property(e => e.EndDate).HasColumnName("endDate");
            builder.Property(e => e.HarvestDate).HasColumnName("harvestDate");
            builder.Property(e => e.PlantingDate).HasColumnName("plantingDate");
            builder.Property(e => e.Quantity).HasColumnName("quantity");
            builder.Property(e => e.StartDate).HasColumnName("startDate");
            builder.Property(e => e.Status).HasColumnName("status");
            builder.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            builder.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchedule19350");

            builder.HasOne(d => d.Crop).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.CropId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchedule700520");

            builder.HasOne(d => d.FarmDetails).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.FarmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchedule407130");
        }
    }
}


