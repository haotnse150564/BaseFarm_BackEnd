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
            builder.ToTable("FarmActivity");

            // Khóa chính
            builder.HasKey(e => e.FarmActivitiesId);

            builder.Property(e => e.FarmActivitiesId)
                   .HasColumnName("farmActivitiesId")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.ActivityType)
                   .HasColumnName("activityType");

            builder.Property(e => e.StartDate)
                   .HasColumnName("startDate");

            builder.Property(e => e.EndDate)
                   .HasColumnName("endDate");

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

            builder.Property(e => e.scheduleId)
                   .HasColumnName("scheduleId");

            // Quan hệ 1-n với Schedule
            builder.HasOne(e => e.Schedule)
                   .WithMany(s => s.FarmActivities) // cần ICollection<FarmActivity> trong Schedule
                   .HasForeignKey(e => e.scheduleId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ n-n với Account thông qua Staff_FarmActivity
            builder.HasMany(e => e.StaffFarmActivities)
                   .WithOne(sfa => sfa.FarmActivity)
                   .HasForeignKey(sfa => sfa.FarmActivityId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với ScheduleLog
            builder.HasMany(e => e.FarmActivityLogsInSchedule)
                   .WithOne(sl => sl.farmActivity) // cần navigation FarmActivity trong ScheduleLog
                   .HasForeignKey(sl => sl.FarmActivityId)
                   .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
