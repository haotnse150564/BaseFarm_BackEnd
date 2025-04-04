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

            builder.Property(e => e.ScheduleId).HasColumnName("scheduleID");
            builder.Property(e => e.AssignedTo).HasColumnName("assignedTo");
            builder.Property(e => e.CreatedAt).HasColumnName("createdAt");
            builder.Property(e => e.CropId).HasColumnName("cropID");
            builder.Property(e => e.EndDate).HasColumnName("endDate");
            builder.Property(e => e.FarmActivityId).HasColumnName("farmActivityID");
            builder.Property(e => e.FarmDetailsId).HasColumnName("farmDetailsID");
            builder.Property(e => e.StartDate).HasColumnName("startDate");
            builder.Property(e => e.Status).HasColumnName("status");
            builder.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            builder.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchedule950653");

            builder.HasOne(d => d.Crop).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.CropId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchedule834236");

            builder.HasOne(d => d.FarmActivity).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.FarmActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchedule760059");

            builder.HasOne(d => d.FarmDetails).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.FarmDetailsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchedule969086");
        }
    }
}


