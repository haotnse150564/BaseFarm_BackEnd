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

            builder.Property(e => e.AccountProfileId)
                .ValueGeneratedNever();
            builder.Property(e => e.CreatedAt);
            builder.Property(e => e.Phone)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.Gender);
            builder.Property(e => e.Images)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.UpdatedAt);

            builder.HasOne(d => d.AccountProfileNavigation).WithOne(p => p.AccountProfile)
                .HasForeignKey<AccountProfile>(d => d.AccountProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}
