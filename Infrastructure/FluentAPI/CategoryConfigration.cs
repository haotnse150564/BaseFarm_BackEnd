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
            builder.Property(e => e.CategoryName)
                .HasMaxLength(255)
                
                .HasColumnName("categoryName")
                .IsUnicode(true);
        }
    }
}
