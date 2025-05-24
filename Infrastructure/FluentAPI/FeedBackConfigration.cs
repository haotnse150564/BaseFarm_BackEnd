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

            builder.HasIndex(e => e.CustomerId, "IX_Feedback_customerID");
            builder.HasKey(e => e.FeedbackId);
            builder.Property(e => e.FeedbackId).ValueGeneratedNever();
            builder.Property(e => e.Comment)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("comment");
            builder.Property(e => e.CreatedAt).HasColumnName("createdAt");
            builder.Property(e => e.CustomerId).HasColumnName("customerID");
            builder.Property(e => e.Rating).HasColumnName("rating");
            builder.Property(e => e.Status).HasColumnName("status");

            builder.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKFeedback770592");

            builder.HasOne(d => d.FeedbackNavigation).WithOne(p => p.Feedback)
                .HasForeignKey<Feedback>(d => d.FeedbackId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.OrderDetail).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKFeedback388276");
        }
    }
}
