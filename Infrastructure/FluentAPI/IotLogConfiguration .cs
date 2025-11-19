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
    public class IotLogConfiguration : IEntityTypeConfiguration<IOTLog>
    {
        public void Configure(EntityTypeBuilder<IOTLog> builder)
        {
            // Tên bảng
            builder.ToTable("IotLogs");

            // Khóa chính
            builder.HasKey(x => x.IotLogId);

            // Cấu hình các thuộc tính
            builder.Property(x => x.IotLogId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.DevicesId)
                .IsRequired();

            builder.Property(x => x.VariableId)
                .HasMaxLength(10);

            builder.Property(x => x.SensorName)
                .HasMaxLength(50);

            builder.Property(x => x.Value)
                .HasColumnType("float") // hoặc "double precision" tùy vào DB
                 .HasColumnName("value");
            builder.Property(x => x.Timestamp)
                .HasColumnType("timestamp without time zone")
                .IsRequired();


            // Quan hệ với bảng Device
            builder.HasOne(x => x.Device)
                .WithMany(d => d.Log) // bạn cần thêm ICollection<IotLog> IotLogs vào class Device
                .HasForeignKey(x => x.DevicesId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
