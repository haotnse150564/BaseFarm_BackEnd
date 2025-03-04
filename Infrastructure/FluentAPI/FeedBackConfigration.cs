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
            builder.Property(e => e.Comment)
                .HasMaxLength(255)
                .IsUnicode(false);
            builder.Property(e => e.CreatedAt);
            builder.Property(e => e.CustomerId);
            builder.Property(e => e.Rating);

            builder.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
