using Domain.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.FluentAPI
{
    public class CropConfigration : IEntityTypeConfiguration<Crop>
    {
        public void Configure(EntityTypeBuilder<Crop> builder)
        {
            builder.HasKey(e => e.CropId);

            builder.ToTable("Crop");

            builder.Property(e => e.CropId);
            builder.Property(e => e.CropName)
                 .HasMaxLength(255)
                 .IsUnicode(false);
            builder.Property(e => e.Description)
                 .HasMaxLength(255)
                 .IsUnicode(false);
            builder.Property(e => e.FarmDetailsId);
            builder.Property(e => e.Fertilizer)
                 .HasMaxLength(255)
                 .IsUnicode(false)
                 .HasColumnName("fertilizer");
            builder.Property(e => e.HarvestDate);
            builder.Property(e => e.Moisture)
                 .HasColumnType("decimal(5, 2)")
                 .HasColumnName("moisture");
            builder.Property(e => e.PlantingDate);
            builder.Property(e => e.Status);
            builder.Property(e => e.Temperature)
                 .HasColumnType("decimal(5, 2)");

            builder.HasOne(d => d.FarmDetails).WithMany(p => p.Crops)
                 .HasForeignKey(d => d.FarmDetailsId);

        }
    }
}
