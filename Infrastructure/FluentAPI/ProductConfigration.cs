using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FluentAPI
{
    public class ProductConfigration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");

            builder.HasKey(e => e.ProductId);
            builder.Property(e => e.CategoryId);
            builder.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)");
            builder.Property(e => e.ProductName);
            builder.Property(e => e.StockQuantity);

            builder.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
