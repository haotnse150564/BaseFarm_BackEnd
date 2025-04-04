using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class OrderConfigration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");

            builder.HasKey(e => e.OrderId);

            builder.Property(e => e.CreatedAt).HasColumnName("createdAt");
            builder.Property(e => e.CustomerId).HasColumnName("customerID");
            builder.Property(e => e.ShippingAddress)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("shippingAddress");
            builder.Property(e => e.Status).HasColumnName("status");
            builder.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("totalPrice");
            builder.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            builder.HasOne(d => d.Customer)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.OrderNavigation)
                .WithOne(p => p.Order)
                .HasForeignKey<Feedback>(d => d.FeedbackId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            ;
        }
    }
}
