using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class IoTDevicesConfigration : IEntityTypeConfiguration<IoTdevice>
    {
        public void Configure(EntityTypeBuilder<IoTdevice> builder)
        {
            builder.HasKey(e => e.IoTdevicesId) ;

            builder.ToTable("IoTDevice");

            builder.Property(e => e.IoTdevicesId) ;
            builder.Property(e => e.DeviceName)
                .HasMaxLength(255)
                .IsUnicode(false) ;
            builder.Property(e => e.DeviceType)
                .HasMaxLength(255)
                .IsUnicode(false) ;
            builder.Property(e => e.ExpiryDate) ;
            builder.Property(e => e.FarmDetailsId);
            builder.Property(e => e.LastUpdate);
            builder.Property(e => e.SensorValue)
                .HasMaxLength(255)
                .IsUnicode(false) ;
            builder.Property(e => e.Status).HasConversion<int>() ;
            builder.Property(e => e.Unit)
                .HasMaxLength(255)
                .IsUnicode(false) ;

            builder.HasOne(d => d.FarmDetails).WithMany(p => p.IoTdevices)
                .HasForeignKey(d => d.FarmDetailsId);

        }
    }
}
