using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FluentAPI
{
    public class FarmEquipmentConfigration : IEntityTypeConfiguration<FarmEquipment>
    {
        public void Configure(EntityTypeBuilder<FarmEquipment> builder)
        {
            builder.ToTable("FarmEquipment");

            builder.HasKey(e => e.FarmEquipmentId);

            builder.Property(e => e.FarmEquipmentId)
                .HasColumnName("farmEquipmentId");

            builder.Property(e => e.AssignDate)
                .HasColumnName("assignDate");

            builder.Property(e => e.RemoveDate)
                .HasColumnName("removeDate");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.Note)
                .HasMaxLength(500)
                .IsUnicode(true)
                .HasColumnName("note");

            builder.Property(e => e.FarmId)
                .HasColumnName("farmId");

            builder.Property(e => e.DeviceId)
                .HasColumnName("deviceId");

            // Quan hệ n-1 với Farm
            builder.HasOne(e => e.Farm)
                .WithMany(f => f.FarmEquipments)
                .HasForeignKey(e => e.FarmId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ n-1 với Device
            builder.HasOne(e => e.Device)
                .WithMany(d => d.FarmEquipments)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
