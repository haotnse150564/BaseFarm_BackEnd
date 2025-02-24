using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FluentAPI
{
    public class FarmDetailConfigration : IEntityTypeConfiguration<FarmDetail>
    {
        public void Configure(EntityTypeBuilder<FarmDetail> builder)
        {
            builder.ToTable("FarmDetail");

            builder.HasKey(e => e.FarmDetailsId);
            builder.Property(e => e.CreatedAt);
            builder.Property(e => e.FarmName)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false)
                    ;
            builder.Property(e => e.StaffId);
            builder.Property(e => e.UpdatedAt);

            builder.HasOne(d => d.Staff).WithMany(p => p.FarmDetails)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
