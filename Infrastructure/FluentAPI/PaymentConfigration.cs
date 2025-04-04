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
            builder.Property(e => e.OrderId);
            builder.Property(e => e.TransactionId);
            builder.Property(e => e.Amount).HasColumnType("decimal(5, 10)");
            builder.Property(e => e.PaymentMethod);
            builder.Property(e => e.VnPayResponseCode);
            builder.Property(e => e.Success);
            builder.Property(e => e.PaymentTime);
            builder.Property(e => e.CreateDate);
            builder.Property(e => e.UpdateDate);
            builder.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
