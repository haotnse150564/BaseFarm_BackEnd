using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.FluentAPI
{
    public class ReportConfigration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Report");

            builder.HasKey(e => e.ReportId);
            builder.Property(e => e.FilePath)
                .HasMaxLength(255)
                .IsUnicode(false);

            builder.HasOne(d => d.GeneratedByNavigation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.GeneratedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
