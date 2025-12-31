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

            builder.HasKey(e => e.ScheduleId);

            builder.Property(e => e.ManagerId)
            .HasColumnName("managerId");

            builder.Property(e => e.ScheduleId)
                .HasColumnName("scheduleId");

            builder.Property(e => e.StartDate)
                .HasColumnName("startDate");

            builder.Property(e => e.EndDate)
                .HasColumnName("endDate");

            builder.Property(e => e.Quantity)
                .HasColumnName("quantity");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.PesticideUsed)
                .HasColumnName("pesticideUsed");

            builder.Property(e => e.DiseaseStatus)
                .HasColumnName("diseaseStatus");

            builder.Property(e => e.currentPlantStage)
                .HasColumnName("currentPlantStage");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            builder.Property(e => e.FarmId)
                .HasColumnName("farmId");

            builder.Property(e => e.CropId)
                .HasColumnName("cropId");

            // Quan hệ n-1 với Account (Manager)
            builder.HasOne(e => e.AssignedToNavigation)
                .WithMany(a => a.Schedules)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ n-1 với Crop
            builder.HasOne(e => e.Crop)
                .WithMany(c => c.Schedules)
                .HasForeignKey(e => e.CropId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ n-1 với Farm
            builder.HasOne(e => e.FarmDetails)
                .WithMany(f => f.Schedules)
                .HasForeignKey(e => e.FarmId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với Inventory
            builder.HasMany(e => e.Inventories)
                .WithOne(i => i.Schedule)
                .HasForeignKey(i => i.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.FarmActivities)
                .WithOne(i => i.Schedule)
                .HasForeignKey(i => i.FarmActivitiesId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}


