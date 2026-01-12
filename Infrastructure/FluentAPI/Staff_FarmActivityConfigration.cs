using Domain.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FluentAPI
{
    public class Staff_FarmActivityConfigration
    {
        public void Configure(EntityTypeBuilder<Staff_FarmActivity> builder)
        {
            builder.ToTable("Staff_FarmActivity");

            builder.HasKey(e => e.Staff_FarmActivityId);

            builder.Property(e => e.Staff_FarmActivityId)
                   .HasColumnName("staffFarmActivityId")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.AccountId)
                   .HasColumnName("accountId");

            builder.Property(e => e.FarmActivityId)
                   .HasColumnName("farmActivityId");

            builder.Property(e => e.CreatedAt)
                   .HasColumnName("createdAt");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updatedAt");

            builder.Property(e => e.CreatedBy)
                .HasColumnName("createdBy");

            builder.Property(e => e.UpdatedBy)
                .HasColumnName("updatedBy");

            builder.HasOne(e => e.Account)
                   .WithMany(a => a.StaffFarmActivities)
                   .HasForeignKey(e => e.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.FarmActivity)
                   .WithMany(f => f.StaffFarmActivities)
                   .HasForeignKey(e => e.FarmActivityId)
                   .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
