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
            builder.Property(e => e.CreatedAt);
            builder.Property(e => e.CustomerId);
            builder.Property(e => e.Status);
            builder.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)");
            builder.Property(e => e.UpdatedAt);

            builder.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
