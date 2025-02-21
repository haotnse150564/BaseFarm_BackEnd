using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentAPI
{
    public class AccountConfigration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            throw new NotImplementedException();
        }
    }

    public class AccountProFileConfigration : IEntityTypeConfiguration<AccountProfile>
    {
        public void Configure(EntityTypeBuilder<AccountProfile> builder)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryConfigration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            throw new NotImplementedException();
        }
    }

    public class FarmActivityConfigration : IEntityTypeConfiguration<FarmActivity>
    {
        public void Configure(EntityTypeBuilder<FarmActivity> builder)
        {
            throw new NotImplementedException();
        }
    }

    public class FarmDetailConfigration : IEntityTypeConfiguration<FarmDetail>
    {
        public void Configure(EntityTypeBuilder<FarmDetail> builder)
        {
            throw new NotImplementedException();
        }
    }

    public class FeedbackConfigration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            throw new NotImplementedException();
        }
    }

    public class IOTDeviceConfigration : IEntityTypeConfiguration<IoTdevice>
    {
        public void Configure(EntityTypeBuilder<IoTdevice> builder)
        {
            throw new NotImplementedException();
        }
    }

    public class OrderConfigration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            throw new NotImplementedException();
        }
    }

    public class OrderDetailConfigration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            throw new NotImplementedException();
        }
    }

    public class PaymentConfigration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            throw new NotImplementedException();
        }
    }
    public class ProductConfigration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            throw new NotImplementedException();
        }
    }
    public class ReportConfigration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            throw new NotImplementedException();
        }
    }
}
