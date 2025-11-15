using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class CategoryConfigration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");

            builder.HasKey(e => e.CategoryId);

            builder.Property(e => e.CategoryId)
                .HasColumnName("categoryId");
            builder.Property(e => e.CategoryTypes)
                .HasColumnName("categoryType");
            builder.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(true) // Cho phép lưu tiếng Việt
                .HasColumnName("categoryName");

            // Quan hệ 1-n với Product
            builder.HasMany(e => e.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với Crop
            builder.HasMany(e => e.Crop)
                .WithOne(c => c.Category)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
