using Domain.Model;
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
            // Tên bảng
            builder.ToTable("FarmActivities");

            // Khóa chính
            builder.HasKey(e => e.FarmActivitiesId);

            builder.Property(e => e.FarmActivitiesId)
                .HasColumnName("farmActivitiesId")
                .ValueGeneratedOnAdd();

            // Enum ActivityType
            builder.Property(e => e.ActivityType)
                .HasColumnName("activityType");

            // StartDate và EndDate (PostgreSQL hỗ trợ Date)
            builder.Property(e => e.StartDate)
                .HasColumnName("startDate")
                .HasColumnType("date");

            builder.Property(e => e.EndDate)
                .HasColumnName("endDate")
                .HasColumnType("date");

            // Status
            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            builder.Property(e => e.createdBy)
                .HasColumnName("createdBy");

            builder.Property(e => e.updatedBy)
                .HasColumnName("updatedBy");

            // Quan hệ 1-n với Schedule
            builder.HasMany(e => e.Schedule)
                .WithOne(s => s.FarmActivities) // giả định Schedule có navigation FarmActivities
                .HasForeignKey(s => s.FarmActivitiesId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
