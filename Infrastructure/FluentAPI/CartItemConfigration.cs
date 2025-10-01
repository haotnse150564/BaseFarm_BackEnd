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
    public class CartItemConfigration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItem");

            builder.HasKey(e => e.CartItemId);

            builder.Property(e => e.CartItemId)
                .HasColumnName("cartItemId");

            builder.Property(e => e.CartId)
                .HasColumnName("cartId");

            builder.Property(e => e.ProductId)
                .HasColumnName("productId");

            builder.Property(e => e.Quantity)
                .HasColumnName("quantity");

            builder.Property(e => e.PriceQuantity)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("priceQuantity");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            // Quan hệ n-1 với Cart
            builder.HasOne(e => e.Cart)
                .WithMany(c => c.CartItems) // giả định Cart có ICollection<CartItem> CartItems
                .HasForeignKey(e => e.CartId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
