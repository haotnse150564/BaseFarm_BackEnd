using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    class DailyLogConfigration : IEntityTypeConfiguration<DailyLog>
    {
        public void Configure(EntityTypeBuilder<DailyLog> builder)
        {
            builder.ToTable("DailyLog");

            builder.HasKey(e => e.TrackingId);
            builder.Property(e => e.ScheduleId).HasColumnName("scheduleID");
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
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Schedule).WithMany(p => p.DailyLogs)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}
