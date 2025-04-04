using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class AccountConfigration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {

            builder.ToTable("Account");

            builder.HasKey(e => e.AccountId);
            builder.Property(e => e.CreatedAt).HasColumnName("createdAt");
            builder.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            builder.Property(e => e.ExpireMinute).HasColumnName("expireMinute");
            builder.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("passwordHash");
            builder.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("refreshToken");
            builder.Property(e => e.Role).HasColumnName("role");
            builder.Property(e => e.Status).HasColumnName("status");
            builder.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
        }
    }
}
