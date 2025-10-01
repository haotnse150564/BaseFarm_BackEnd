using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class FeedBackConfigration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedback");

            builder.HasKey(e => e.FeedbackId);

            builder.Property(e => e.FeedbackId)
                .HasColumnName("feedbackId");

            builder.Property(e => e.Comment)
                .HasMaxLength(500)
                .IsUnicode(true) // Cho phép bình luận bằng tiếng Việt
                .HasColumnName("comment");

            builder.Property(e => e.Rating)
                .HasColumnName("rating");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("createdAt");

            builder.Property(e => e.CustomerId)
                .HasColumnName("customerId");

            builder.Property(e => e.OrderDetailId)
                .HasColumnName("orderDetailId");

            // Quan hệ n-1 với Account (Customer)
            builder.HasOne(e => e.Customer)
                .WithMany(a => a.Feedbacks)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ n-1 với OrderDetail
            builder.HasOne(e => e.OrderDetail)
                .WithMany(od => od.Feedbacks)
                .HasForeignKey(e => e.OrderDetailId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
