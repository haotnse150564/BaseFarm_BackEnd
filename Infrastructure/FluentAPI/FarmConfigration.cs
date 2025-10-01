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
    class FarmConfigration : IEntityTypeConfiguration<Farm>
    {
        public void Configure(EntityTypeBuilder<Farm> builder)
        {
            builder.ToTable("Farm");

            builder.HasKey(e => e.FarmId);

            builder.Property(e => e.FarmId)
                .HasColumnName("farmId");

            builder.Property(e => e.FarmName)
                .HasMaxLength(100)
                .IsUnicode(true) // Cho phép tên nông trại có tiếng Việt
                .HasColumnName("farmName");

            builder.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(true)
                .HasColumnName("location");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            builder.Property(e => e.AccountId)
                .HasColumnName("accountId");

            // Quan hệ n-1 với Account
            builder.HasOne(e => e.Account)
                .WithMany(a => a.Farms)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với Schedule
            builder.HasMany(e => e.Schedules)
                .WithOne(s => s.FarmDetails)
                .HasForeignKey(s => s.FarmId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với FarmEquipment
            builder.HasMany(e => e.FarmEquipments)
                .WithOne(fe => fe.Farm)
                .HasForeignKey(fe => fe.FarmId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
