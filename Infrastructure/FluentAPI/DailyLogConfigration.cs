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
    class DailyLogConfigration : IEntityTypeConfiguration<DailyLog>
    {
        public void Configure(EntityTypeBuilder<DailyLog> builder)
        {
            builder.ToTable("DailyLog");

            builder.Property(e => e.TrackingId)
                .ValueGeneratedNever()
                .HasColumnName("trackingID");
            builder.Property(e => e.AssignedTo).HasColumnName("assignedTo");
            builder.Property(e => e.CreatedAt).HasColumnName("createdAt");
            builder.Property(e => e.Date).HasColumnName("date");
            builder.Property(e => e.Notes)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("notes");
            builder.Property(e => e.Status).HasColumnName("status");
            builder.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            builder.HasOne(d => d.AssignedToNavigation).WithMany(p => p.DailyLogs)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDailyLog 60639");

            builder.HasOne(d => d.Tracking).WithOne(p => p.DailyLog)
                .HasForeignKey<DailyLog>(d => d.TrackingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKDailyLog 386238");
        }
    }
}
