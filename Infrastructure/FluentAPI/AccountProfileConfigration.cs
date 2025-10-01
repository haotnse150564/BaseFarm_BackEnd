using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class AccountProfileConfigration : IEntityTypeConfiguration<AccountProfile>
    {
        public void Configure(EntityTypeBuilder<AccountProfile> builder)
        {
            builder.ToTable("AccountProfile");

            builder.HasKey(e => e.AccountProfileId);

            builder.Property(e => e.AccountProfileId)
                .HasColumnName("accountProfileId");

            builder.Property(e => e.Gender)
                .HasColumnName("gender");

            builder.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false) // Số điện thoại không cần Unicode
                .HasColumnName("phone");

            builder.Property(e => e.Fullname)
                .HasMaxLength(75)
                .IsUnicode(true) // Cho phép lưu tiếng Việt
                .HasColumnName("fullname");

            builder.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(true)
                .HasColumnName("address");

            builder.Property(e => e.Images)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("images");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            // Quan hệ 1-1 với Account
            builder.HasOne(e => e.AccountProfileNavigation)
                .WithOne(a => a.AccountProfile)
                .HasForeignKey<AccountProfile>(e => e.AccountProfileId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
