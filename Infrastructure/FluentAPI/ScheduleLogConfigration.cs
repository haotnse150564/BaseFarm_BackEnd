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
    class ScheduleLogConfigration : IEntityTypeConfiguration<ScheduleLog>
    {
        public void Configure(EntityTypeBuilder<ScheduleLog> builder)
        {
            // Tên bảng
            builder.ToTable("ScheduleLog");

            // Khóa chính
            builder.HasKey(e => e.ScheduleLogId);

            builder.Property(e => e.ScheduleLogId)
                   .HasColumnName("scheduleLogId")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.ScheduleId)
                   .HasColumnName("scheduleId");

            builder.Property(e => e.FarmActivityId)
                   .HasColumnName("farmActivityId");

            builder.Property(e => e.Notes)
                   .HasColumnName("notes")
                   .HasMaxLength(500); // tuỳ chỉnh độ dài

            builder.Property(e => e.CreatedAt)
                   .HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt)
                   .HasColumnName("updatedAt");

            builder.Property(e => e.CreatedBy)
                   .HasColumnName("createdBy");

            builder.Property(e => e.UpdatedBy)
                   .HasColumnName("updatedBy");

            // Quan hệ n-1 với Schedule
            builder.HasOne(e => e.schedule)
                   .WithMany(s => s.ScheduleLog) // cần ICollection<ScheduleLog> trong Schedule
                   .HasForeignKey(e => e.ScheduleId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ n-1 với FarmActivity
            builder.HasOne(e => e.farmActivity)
                   .WithMany(f => f.FarmActivityLogsInSchedule) // cần ICollection<ScheduleLog> trong FarmActivity
                   .HasForeignKey(e => e.FarmActivityId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
