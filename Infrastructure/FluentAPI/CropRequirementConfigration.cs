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
            builder.ToTable("CropRequirement");

            builder.HasKey(e => e.RequirementId);

            builder.Property(e => e.RequirementId)
                .HasColumnName("requirementId");

            builder.Property(e => e.PlantStage)
                .HasColumnName("plantStage");

            builder.Property(e => e.EstimatedDate)
                .HasColumnName("estimatedDate");

            builder.Property(e => e.Moisture)
                .HasColumnType("decimal(5,2)") // Tùy chỉnh độ chính xác nếu cần
                .HasColumnName("moisture");

            builder.Property(e => e.Temperature)
                .HasColumnType("decimal(5,2)")
                .HasColumnName("temperature");

            builder.Property(e => e.Fertilizer)
                .HasMaxLength(255)
                .IsUnicode(true) // Cho phép lưu tên phân bón bằng tiếng Việt
                .HasColumnName("fertilizer");

            // Quan hệ 1-1 với Crop
            builder.HasOne(e => e.Crop)
                .WithOne(c => c.CropRequirement)
                .HasForeignKey<CropRequirement>(e => e.RequirementId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
