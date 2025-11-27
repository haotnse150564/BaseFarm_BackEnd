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
    class CropRequirementConfigration : IEntityTypeConfiguration<CropRequirement>
    {
        public void Configure(EntityTypeBuilder<CropRequirement> builder)
        {
            // Table name
            builder.ToTable("CropRequirements");

            // Primary key
            builder.HasKey(cr => cr.CropRequirementId);

            // Properties
            builder.Property(cr => cr.CropRequirementId)
                   .HasColumnName("CropRequirementId")
                   .ValueGeneratedOnAdd();

            builder.Property(cr => cr.CropId)
                   .IsRequired();

            builder.Property(cr => cr.PlantStage)
                   .HasConversion<int>() // Enum -> int
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
                   .HasColumnType("timestamp")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(cr => cr.UpdatedDate)
                   .HasColumnType("timestamp");

            builder.Property(cr => cr.IsActive)
                   .HasColumnType("boolean")
                   .HasDefaultValue(true);

            // Relationships
            builder.HasOne(cr => cr.Crop)
                   .WithMany(c => c.CropRequirement)
                   .HasForeignKey(cr => cr.CropId)
                   .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
