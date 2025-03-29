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
            builder.HasKey(e => e.ScheduleId);

            builder.ToTable("Schedule");

            builder.Property(e => e.ScheduleId);
            builder.Property(e => e.AssignedTo);
            builder.Property(e => e.CreatedAt);
            builder.Property(e => e.EndDate);
            builder.Property(e => e.FarmActivityId);
            builder.Property(e => e.FarmDetailsId);
            builder.Property(e => e.StartDate);
            builder.Property(e => e.Status).HasConversion<int>();
            builder.Property(e => e.UpdatedAt);

            builder.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.AssignedTo);

            builder.HasOne(d => d.FarmActivity).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.FarmActivityId);

            builder.HasOne(d => d.FarmDetails).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.FarmDetailsId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}


