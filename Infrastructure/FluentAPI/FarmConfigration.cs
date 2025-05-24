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
    class FarmConfigration : IEntityTypeConfiguration<Farm>
    {
        public void Configure(EntityTypeBuilder<Farm> builder)
        {
            builder.ToTable("Farm");

            builder.HasIndex(e => e.AccountId, "IX_Farm_accountID");
            builder.HasKey(e => e.FarmId);
            builder.Property(e => e.AccountId).HasColumnName("accountID");
            builder.Property(e => e.CreatedAt).HasColumnName("createdAt");
            builder.Property(e => e.FarmName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("farmName");
            builder.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("location");
            builder.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            builder.HasOne(d => d.Account).WithMany(p => p.Farms)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKFarm576533");
        }
    }
}
