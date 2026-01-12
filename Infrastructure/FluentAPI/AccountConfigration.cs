using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class AccountConfigration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {

            builder.ToTable("Account");

            builder.HasKey(e => e.AccountId);

            builder.Property(e => e.AccountId)
                .HasColumnName("accountId");

            builder.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(true) // Cho phép lưu tiếng Việt
                .HasColumnName("email");

            builder.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("passwordHash");

            builder.Property(e => e.Role)
                .HasColumnName("role");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            // Quan hệ 1-1 với AccountProfile
            builder.HasOne(e => e.AccountProfile)
                .WithOne(p => p.AccountProfileNavigation)
                .HasForeignKey<AccountProfile>(p => p.AccountProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với Farm
            builder.HasMany(e => e.Farms)
                .WithOne(f => f.Account)
                .HasForeignKey(f => f.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với Feedback
            builder.HasMany(e => e.Feedbacks)
                .WithOne(f => f.Customer)
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với Order
            builder.HasMany(e => e.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n với Schedule
            builder.HasMany(e => e.Schedules)
                .WithOne(s => s.AssignedToNavigation)
                .HasForeignKey(s => s.ManagerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.Address)
                .WithOne(ad => ad.Account)
                .HasForeignKey(ad => ad.AddressID)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(a => a.StaffFarmActivities)
                .WithOne(fa => fa.Account)
                .HasForeignKey(fa => fa.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
