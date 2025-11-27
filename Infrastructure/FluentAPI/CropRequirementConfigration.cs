using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class CropRequirementConfiguration : IEntityTypeConfiguration<CropRequirement>
    {
        public void Configure(EntityTypeBuilder<CropRequirement> builder)
        {
            builder.ToTable("CropRequirements");

            builder.HasKey(cr => cr.CropRequirementId);

            builder.Property(cr => cr.CropRequirementId)
                   .ValueGeneratedOnAdd();

            builder.Property(cr => cr.CropId)
                   .IsRequired();

            builder.Property(cr => cr.PlantStage)
                   .HasConversion<int>()
                   .HasColumnType("integer");

            builder.Property(cr => cr.EstimatedDate)
                   .HasColumnType("integer");

            builder.Property(cr => cr.Moisture)
                   .HasColumnType("numeric(10,2)");

            builder.Property(cr => cr.Temperature)
                   .HasColumnType("numeric(10,2)");

            builder.Property(cr => cr.Fertilizer)
                   .HasColumnType("text");

            builder.Property(cr => cr.LightRequirement)
                   .HasColumnType("numeric(10,2)");

            builder.Property(cr => cr.WateringFrequency)
                   .HasColumnType("text");

            builder.Property(cr => cr.Notes)
                   .HasColumnType("text");

            builder.Property(cr => cr.CreatedDate)
                  .HasConversion(
                      v => v.ToDateTime(TimeOnly.MinValue),   // DateOnly -> DateTime
                      v => DateOnly.FromDateTime(v)           // DateTime -> DateOnly
                  )
                  .HasColumnType("date")                      // PostgreSQL 'date'
                  .HasDefaultValueSql("CURRENT_DATE");        // mặc định ngày hiện tại

            builder.Property(cr => cr.UpdatedDate)
                   .HasConversion(
                       v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : default,   // DateOnly? -> DateTime?
                       v => v != default ? DateOnly.FromDateTime(v) : (DateOnly?)null       // DateTime? -> DateOnly?
                   )
                   .HasColumnType("date");

            // Quan hệ
            builder.HasOne(cr => cr.Crop)
                   .WithMany(c => c.CropRequirement) // property trong Crop phải là ICollection<CropRequirement>
                   .HasForeignKey(cr => cr.CropId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}