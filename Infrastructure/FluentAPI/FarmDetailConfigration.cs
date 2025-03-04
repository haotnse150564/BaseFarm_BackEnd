using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class FarmDetailConfigration : IEntityTypeConfiguration<FarmDetail>
    {
        public void Configure(EntityTypeBuilder<FarmDetail> builder)
        {
            builder.HasKey(e => e.FarmDetailsId);

            builder.ToTable("FarmDetail");

            builder.Property(e => e.FarmDetailsId);
            builder.Property(e => e.AccountId);
            builder.Property(e => e.CreatedAt);
            builder.Property(e => e.FarmName)
                  .HasMaxLength(255)
                  .IsUnicode(false);
            builder.Property(e => e.Location)
                  .HasMaxLength(255)
                  .IsUnicode(false);
            builder.Property(e => e.UpdatedAt);

            builder.HasOne(d => d.Account).WithMany(p => p.FarmDetails)
                      .HasForeignKey(d => d.AccountId);
            builder.HasMany(a => a.Schedules).WithOne(p => p.FarmDetails)
                .HasForeignKey(d => d.FarmDetailsId);
        }
    }
}
