﻿using Domain.Model;
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

            builder.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPayment513267");
        }
    }
}
