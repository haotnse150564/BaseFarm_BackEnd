using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class IoTDevicesConfigration : IEntityTypeConfiguration<IoTdevice>
    {
        public void Configure(EntityTypeBuilder<IoTdevice> builder)
        {
            builder.ToTable("IoTDevice");

            builder.HasKey(e => e.IoTdevicesId);
            builder.Property(e => e.DeviceName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("deviceName");
            builder.Property(e => e.DeviceType)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("deviceType");
            builder.Property(e => e.ExpiryDate).HasColumnName("expiryDate");
            builder.Property(e => e.FarmDetailsId).HasColumnName("farmDetailsID");
            builder.Property(e => e.LastUpdate).HasColumnName("lastUpdate");
            builder.Property(e => e.SensorValue)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("sensorValue");
            builder.Property(e => e.Status).HasColumnName("status");
            builder.Property(e => e.Unit)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("unit");

            builder.HasOne(d => d.FarmDetails).WithMany(p => p.IoTdevices)
                .HasForeignKey(d => d.FarmDetailsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                ;

        }
    }
}
