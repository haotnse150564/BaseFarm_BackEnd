using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class IoTDevicesConfigration : IEntityTypeConfiguration<IoTdevice>
    {
        public void Configure(EntityTypeBuilder<IoTdevice> builder)
        {
            builder.ToTable("IoTDevices");

            builder.HasKey(e => e.IoTdevicesId);
            builder.Property(e => e.DeviceName);
            builder.Property(e => e.DeviceType)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.FarmId);
            builder.Property(e => e.LastUpdate);
            builder.Property(e => e.SensorValue)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.Status);
            builder.Property(e => e.Unit)
                .HasMaxLength(255)
                .IsUnicode(false);

            builder.HasOne(d => d.Farm).WithMany(p => p.IoTdevices)
                .HasForeignKey(d => d.FarmId)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}
