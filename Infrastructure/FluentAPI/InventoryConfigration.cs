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

            builder.Property(e => e.InventoryId).HasColumnName("inventoryID");
            builder.Property(e => e.ExpiryDate).HasColumnName("expiryDate");
            builder.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("location");
            builder.Property(e => e.ProductId).HasColumnName("productID");
            builder.Property(e => e.Status).HasColumnName("status");
            builder.Property(e => e.StockQuantity).HasColumnName("stockQuantity");

            builder.HasOne(d => d.Product).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKInventory114462");
        }
    }
}
