using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class OrderDetailConfigration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {

            builder.ToTable("OrderDetail");

            builder.HasKey(e => e.OrderDetailId);
            builder.Property(e => e.OrderId);
            builder.Property(e => e.ProductId);
            builder.Property(e => e.Quantity);
            builder.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)");

            builder.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
