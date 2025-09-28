using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class PaymentConfigration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payment");
            builder.HasKey(e => e.PaymentId);

            builder.HasIndex(e => e.OrderId, "IX_Payment_OrderId");

            builder.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            builder.Property(e => e.CreateDate)
                .HasColumnType("timestamp with time zone")
              .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.PaymentTime)
             .HasColumnType("timestamp with time zone")
           .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.UpdateDate)
             .HasColumnType("timestamp with time zone")
           .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPayment513267");
        }
    }
}
