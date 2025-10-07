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
            .HasColumnName("addressID")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.CustomerID)
            .HasColumnName("customerID")
            .IsRequired();

            builder.Property(e => e.Province)
            .HasColumnName("province")
            .HasMaxLength(100)
            .IsUnicode(true);

            builder.Property(e => e.District)
            .HasColumnName("district")
            .HasMaxLength(100)
            .IsUnicode(true);

            builder.Property(e => e.Ward)
            .HasColumnName("ward")
            .HasMaxLength(100)
            .IsUnicode(true);

            builder.Property(e => e.Street)
            .HasColumnName("street")
            .HasMaxLength(200)
            .IsUnicode(true);

            builder.Property(e => e.IsDefault)
            .HasColumnName("isDefault")
            .HasDefaultValue(false);

            builder.Property(e => e.Latitude)
            .HasColumnName("latitude")
             .HasColumnType("decimal(9,6)");

            builder.Property(e => e.Longitude)
            .HasColumnType("decimal(9,6)");

            builder.Property(e => e.CreatedAt).HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            // Optional: Navigation property
            builder.HasOne(e => e.Account)
            .WithMany()
            .HasForeignKey(e => e.CustomerID)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
