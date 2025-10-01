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

            builder.Property(e => e.CropId)
                .HasColumnName("cropId");

            builder.Property(e => e.CropName)
                .HasMaxLength(100)
                .IsUnicode(true) // Cho phép lưu tên cây bằng tiếng Việt
                .HasColumnName("cropName");

            builder.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(true)
                .HasColumnName("description");

            builder.Property(e => e.Origin)
                .HasMaxLength(100)
                .IsUnicode(true)
                .HasColumnName("origin");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.CreateAt)
                .HasColumnName("createAt");

            builder.Property(e => e.UpdateAt)
                .HasColumnName("updateAt");

            builder.Property(e => e.CategoryId)
                .HasColumnName("categoryId");

            // Quan hệ n-1 với Category
            builder.HasOne(e => e.Category)
                .WithMany(c => c.Crop)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            //// Quan hệ 1-1 với CropRequirement
            //builder.HasOne(e => e.CropRequirement)
            //    .WithOne(r => r.Crop)
            //    .HasForeignKey<CropRequirement>(r => r.CropId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Quan hệ 1-1 với Product
            //builder.HasOne(e => e.Product)
            //    .WithOne(p => p.Crop)
            //    .HasForeignKey<Product>(p => p.CropId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với Schedule
            builder.HasMany(e => e.Schedules)
                .WithOne(s => s.Crop)
                .HasForeignKey(s => s.CropId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
