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
    class InventoryConfigration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable("Inventory");

            builder.HasKey(e => e.InventoryId);

            builder.Property(e => e.InventoryId)
                .HasColumnName("inventoryId");

            builder.Property(e => e.ItemType)
                .HasMaxLength(100)
                .IsUnicode(true) // Cho phép lưu loại vật phẩm bằng tiếng Việt
                .HasColumnName("itemType");

            builder.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(true)
                .HasColumnName("location");

            builder.Property(e => e.StockQuantity)
                .HasColumnName("stockQuantity");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.ExpireDate)
                .HasColumnName("expireDate");

            builder.Property(e => e.CreateAt)
                .HasColumnName("createAt");

            builder.Property(e => e.ProductId)
                .HasColumnName("productId");

            builder.Property(e => e.ScheduleId)
                .HasColumnName("scheduleId");

            // Quan hệ n-1 với Product
            builder.HasOne(e => e.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ n-1 với Schedule
            builder.HasOne(e => e.Schedule)
                .WithMany(s => s.Inventories)
                .HasForeignKey(e => e.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
