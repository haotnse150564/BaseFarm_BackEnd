using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class DevicesConfigration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.ToTable("Devices");

            builder.HasKey(e => e.DevicessId);

            builder.Property(e => e.DevicessId)
                .HasColumnName("devicessId");

            builder.Property(e => e.DeviceName)
                .HasMaxLength(100)
                .IsUnicode(true) // Cho phép tên thiết bị có tiếng Việt
                .HasColumnName("deviceName");

            builder.Property(e => e.DeviceType)
                .HasMaxLength(100)
                .IsUnicode(true)
                .HasColumnName("deviceType");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.LastUpdate)
                .HasColumnName("lastUpdate");

            builder.Property(e => e.ExpiryDate)
                .HasColumnName("expiryDate");

            builder.Property(e => e.FarmDetailsId)
                .HasColumnName("farmDetailsId");

            //// Quan hệ n-1 với Farm
            //builder.HasOne(e => e.FarmDetails)
            //    .WithMany(f => f.D) // giả định Farm có ICollection<Devicess> Devices
            //    .HasForeignKey(e => e.FarmDetailsId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Quan hệ 1-n với FarmEquipment
            //builder.HasMany(e => e.FarmEquipments)
            //    .WithOne(fe => fe.Device)
            //    .HasForeignKey(fe => fe.DevicessId)
            //    .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
