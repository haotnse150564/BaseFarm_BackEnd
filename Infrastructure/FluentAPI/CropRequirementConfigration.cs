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
            builder.HasKey(e => e.RequirementId);

            builder.ToTable("CropRequirement");


            builder.Property(e => e.RequirementId).ValueGeneratedNever();
            builder.Property(e => e.EstimatedDate).HasColumnName("estimatedDate");
            builder.Property(e => e.Fertilizer)
                .HasMaxLength(255)
                
                .HasColumnName("fertilizer");
            builder.Property(e => e.Moisture)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("moisture");
            builder.Property(e => e.Temperature)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("temperature");


            builder.HasOne(d => d.Requirement).WithOne(p => p.CropRequirement)
                .HasForeignKey<CropRequirement>(d => d.RequirementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKCropRequir719183");
        }
    }
}
