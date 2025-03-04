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
            builder.Property(e => e.CreatedAt);
            builder.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                ;                                                       
            builder.Property(e => e.Email);
            builder.Property(e => e.Role);
            builder.Property(e => e.Status);
            builder.Property(e => e.UpdatedAt);
        }
    }
}
