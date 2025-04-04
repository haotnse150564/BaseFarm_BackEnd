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

            builder.Property(e => e.CropId).HasColumnName("cropID");
            builder.Property(e => e.CropName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cropName");
            builder.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            builder.Property(e => e.HarvestDate).HasColumnName("harvestDate");
            builder.Property(e => e.PlantingDate).HasColumnName("plantingDate");
            builder.Property(e => e.Quantity).HasColumnName("quantity");
            builder.Property(e => e.Status).HasColumnName("status");

        }
    }
}
