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

            builder.HasIndex(e => e.OrderId, "IX_OrderDetail_orderID");

            builder.HasIndex(e => e.ProductId, "IX_OrderDetail_productID");
            builder.HasKey(e => e.OrderDetailId);
            builder.Property(e => e.OrderId).HasColumnName("orderID");
            builder.Property(e => e.ProductId).HasColumnName("productID");
            builder.Property(e => e.Quantity).HasColumnName("quantity");
            builder.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unitPrice");

            builder.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrderDetai876065");

            builder.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrderDetai274486");
        }
    }
}
