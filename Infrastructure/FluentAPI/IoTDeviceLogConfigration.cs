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
    class IoTDeviceLogConfigration : IEntityTypeConfiguration<IoTdeviceLog>
    {
        public void Configure(EntityTypeBuilder<IoTdeviceLog> builder)
        {
            builder.ToTable("IoTDeviceLog");

            builder.HasKey(e => e.LogId);
            builder.Property(e => e.CreatedAt).HasColumnName("createdAt");
            builder.Property(e => e.IoTdevice).HasColumnName("ioTDevice");
            builder.Property(e => e.TrackingId).HasColumnName("trackingID");

            builder.HasOne(d => d.IoTdeviceNavigation).WithMany(p => p.IoTdeviceLogs)
                .HasForeignKey(d => d.IoTdevice)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKIoTDeviceL440028");

            builder.HasOne(d => d.Tracking).WithMany(p => p.IoTdeviceLogs)
                .HasForeignKey(d => d.TrackingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                ;
        }
    }
}
