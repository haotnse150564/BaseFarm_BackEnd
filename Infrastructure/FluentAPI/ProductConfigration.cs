using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class ProductConfigration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            #region Cũ
            //builder.ToTable("Product");

            //builder.HasIndex(e => e.CategoryId, "IX_Product_categoryID");
            //builder.Property(e => e.ProductId).ValueGeneratedNever();
            //builder.Property(e => e.CategoryId).HasColumnName("categoryID");
            //builder.Property(e => e.Description)
            //    .HasMaxLength(255)
            //    .IsUnicode(false)
            //    .HasColumnName("description");
            //builder.Property(e => e.Images)
            //    .HasMaxLength(255)
            //    .IsUnicode(false)
            //    .HasColumnName("images");
            //builder.Property(e => e.Price)
            //    .HasColumnType("decimal(10, 2)")
            //    .HasColumnName("price");
            //builder.Property(e => e.ProductName)
            //    .HasMaxLength(255)
            //    .IsUnicode(false)
            //    .HasColumnName("productName");
            //builder.Property(e => e.Status).HasColumnName("status");
            //builder.Property(e => e.StockQuantity).HasColumnName("stockQuantity");

            //builder.HasOne(d => d.Category).WithMany(p => p.Products)
            //    .HasForeignKey(d => d.CategoryId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FKProduct896000");

            //builder.HasOne(d => d.ProductNavigation).WithOne(p => p.Product)
            //    .HasForeignKey<Product>(d => d.ProductId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FKProduct309661");
            #endregion
            #region mới
            builder.ToTable("Product");

            builder.HasKey(e => e.ProductId);

            builder.Property(e => e.ProductId)
                .HasColumnName("productId");

            builder.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(true) // Cho phép tên sản phẩm có tiếng Việt
                .HasColumnName("productName");

            builder.Property(e => e.Images)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("images");

            builder.Property(e => e.Price)
                .HasColumnType("decimal(18,2)") // Tùy chỉnh độ chính xác nếu cần
                .HasColumnName("price");

            builder.Property(e => e.StockQuantity)
                .HasColumnName("stockQuantity");

            builder.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(true)
                .HasColumnName("description");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            builder.Property(e => e.CategoryId)
                .HasColumnName("categoryId");

            // Quan hệ n-1 với Category
            builder.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ 1-1 với Crop
            builder.HasOne(e => e.ProductNavigation)
                .WithOne(c => c.Product)
                .HasForeignKey<Crop>(c => c.CropId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với Inventory
            builder.HasMany(e => e.Inventories)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với OrderDetail
            builder.HasMany(e => e.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
            #endregion
        }
    }
}
