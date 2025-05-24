using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class ProductConfigration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");

            builder.HasIndex(e => e.CategoryId, "IX_Product_categoryID");
            builder.Property(e => e.ProductId).ValueGeneratedNever();
            builder.Property(e => e.CategoryId).HasColumnName("categoryID");
            builder.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            builder.Property(e => e.Images)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("images");
            builder.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            builder.Property(e => e.ProductName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("productName");
            builder.Property(e => e.Status).HasColumnName("status");
            builder.Property(e => e.StockQuantity).HasColumnName("stockQuantity");

            builder.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProduct896000");

            builder.HasOne(d => d.ProductNavigation).WithOne(p => p.Product)
                .HasForeignKey<Product>(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProduct309661");
        }
    }
}
