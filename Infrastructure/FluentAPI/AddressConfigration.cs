using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FluentAPI
{
    class AddressConfigration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");
            builder.HasKey(e => e.AddressID);

            builder.Property(e => e.AddressID)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.CustomerID)
            .IsRequired();

            builder.Property(e => e.Province)
            .HasMaxLength(100)
            .IsUnicode(true);

            builder.Property(e => e.District)
            .HasMaxLength(100)
            .IsUnicode(true);

            builder.Property(e => e.Ward)
            .HasMaxLength(100)
            .IsUnicode(true);

            builder.Property(e => e.Street)
            .HasMaxLength(200)
            .IsUnicode(true);

            builder.Property(e => e.IsDefault)
            .HasDefaultValue(false);

            builder.Property(e => e.Latitude)
            .HasColumnType("decimal(9,6)");

            builder.Property(e => e.Longitude)
            .HasColumnType("decimal(9,6)");

            builder.Property(e => e.CreatedAt)
            .HasColumnType("datetime");

            builder.Property(e => e.UpdatedAt)
            .HasColumnType("datetime");

            // Optional: Navigation property
            builder.HasOne(e => e.Account)
            .WithMany()
            .HasForeignKey(e => e.CustomerID)
            .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
