using Domain.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.FluentAPI
{
    public class CropConfigration : IEntityTypeConfiguration<Crop>
    {
        public void Configure(EntityTypeBuilder<Crop> builder)
        {
            builder.ToTable("Crop");

            builder.HasKey(e => e.CropId); 
            builder.Property(e => e.CategoryId);
            builder.Property(e => e.CropName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cropName");
            builder.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            builder.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("imageUrl");
            builder.Property(e => e.Origin)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("origin");
            builder.Property(e => e.Status).HasColumnName("status");

            builder.HasOne(d => d.Category).WithMany(p => p.Crops)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKCrop824568");

        }
    }
}
