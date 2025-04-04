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
                .ValueGeneratedNever()
                .HasColumnName("accountProfileID");
            builder.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            builder.Property(e => e.CreatedAt).HasColumnName("createdAt");
            builder.Property(e => e.Fullname)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("fullname");
            builder.Property(e => e.Gender).HasColumnName("gender");
            builder.Property(e => e.Images)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("images");
            builder.Property(e => e.Phone)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("phone");
            builder.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            builder.HasOne(d => d.AccountProfileNavigation).WithOne(p => p.AccountProfile)
                .HasForeignKey<AccountProfile>(d => d.AccountProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAccountPro821701");
        }
    }
}
