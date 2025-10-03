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
    public class CartConfigration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Cart");

            builder.HasKey(e => e.CartId);

            builder.Property(e => e.CartId)
                .HasColumnName("cartId");

            builder.Property(e => e.CustomerId)
                .HasColumnName("customerId");

            builder.Property(e => e.PaymentStatus)
                .HasColumnName("paymentStatus");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            builder.Property(e => e.ExpereAt)
                .HasColumnName("expereAt");

            // Quan hệ n-1 với Account (Customer)
            builder.HasOne(e => e.Customer)
                .WithMany(a => a.Cart) // giả định Account có ICollection<Cart> Carts
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
