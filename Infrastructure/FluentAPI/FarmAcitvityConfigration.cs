using Domain;
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

            builder.HasKey(e => e.FarmActivitiesId);
            builder.Property(e => e.ActivityType);
            builder.Property(e => e.AssignedTo);
            builder.Property(e => e.EndDate);
            builder.Property(e => e.FarmId);
            builder.Property(e => e.StartDate);
            builder.Property(e => e.Status);

            builder.HasOne(d => d.AssignedToNavigation).WithMany(p => p.FarmActivities)
                .HasForeignKey(d => d.AssignedTo);

            builder.HasOne(d => d.Farm).WithMany(p => p.FarmActivities)
                .HasForeignKey(d => d.FarmId);
        }
    }
}
