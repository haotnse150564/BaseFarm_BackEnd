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
            builder.HasKey(e => e.CropLogId);

            builder.Property(e => e.CropLogId)
                .HasColumnName("cropLogId")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.ScheduleId)
                .HasColumnName("scheduleId");

            // Notes có thể viết bằng tiếng Việt có dấu
            builder.Property(e => e.Notes)
                .HasColumnName("notes")
                .HasColumnType("text"); 

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
                .WithMany(s => s.ScheduleLog) // giả định Schedule có ICollection<CropLog>
                .HasForeignKey(e => e.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
