using Azure.Core;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        //{
        //    optionBuilder.UseSqlServer("ConnectionsString");
        //}
        public DbSet<Account> Account { get; set; }

        public DbSet<AccountProfile> AccountProfile { get; set; }

        public DbSet<Category> Categorie { get; set; }

        public DbSet<Crop> Crop { get; set; }

        public DbSet<FarmActivity> FarmActivitie { get; set; }

        public DbSet<FarmDetail> FarmDetail { get; set; }

        public DbSet<Feedback> Feedback { get; set; }

        public DbSet<IoTdevice> IoTdevice { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderDetail> OrderDetail { get; set; }

        public DbSet<Payment> Payment { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<Report> Report { get; set; }

        public DbSet<Schedule> Schedule { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            #region seedingdata
            builder.Entity<Account>().HasData(
                new Account { AccountId = 1, Email = "admin@email.com", PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", Status = 1, Role = Domain.Enum.Roles.Admin, RefreshToken = null, ExpireMinutes = 30, CreatedAt = DateOnly.Parse("3/1/25"), UpdatedAt = DateOnly.Parse("3/1/25") },
                new Account { AccountId = 2, Email = "manager@email.com", PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", Status = 1, Role = Domain.Enum.Roles.Manager, RefreshToken = null, ExpireMinutes = 30, CreatedAt = DateOnly.Parse("1/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new Account { AccountId = 3, Email = "staff01@email.com", PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", Status = 1, Role = Domain.Enum.Roles.Staff, RefreshToken = null, ExpireMinutes = 30, CreatedAt = DateOnly.Parse("5/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new Account { AccountId = 6, Email = "staff02@email.com", PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", Status = 1, Role = Domain.Enum.Roles.Staff, RefreshToken = null, ExpireMinutes = 30, CreatedAt = DateOnly.Parse("5/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new Account { AccountId = 9, Email = "staff03@email.com", PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", Status = 1, Role = Domain.Enum.Roles.Staff, RefreshToken = null, ExpireMinutes = 30, CreatedAt = DateOnly.Parse("5/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new Account { AccountId = 4, Email = "cus01@email.com", PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", Status = 1, Role = Domain.Enum.Roles.Customer, RefreshToken = null, ExpireMinutes = 30, CreatedAt = DateOnly.Parse("4/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new Account { AccountId = 8, Email = "cus03@email.com", PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", Status = 1, Role = Domain.Enum.Roles.Customer, RefreshToken = null, ExpireMinutes = 30, CreatedAt = DateOnly.Parse("4/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new Account { AccountId = 7, Email = "cus04@email.com", PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", Status = 1, Role = Domain.Enum.Roles.Customer, RefreshToken = null, ExpireMinutes = 30, CreatedAt = DateOnly.Parse("4/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new Account { AccountId = 5, Email = "cus02@email.com", PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", Status = 1, Role = Domain.Enum.Roles.Customer, RefreshToken = null, ExpireMinutes = 30, CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") }
            );
            builder.Entity<AccountProfile>().HasData(
                new AccountProfile { AccountProfileId = 1, Gender = 1, Phone = "0123456789", Fullname = "Admin", Address = "HCM", Images = null, CreatedAt = DateOnly.Parse("3/1/25"), UpdatedAt = DateOnly.Parse("3/1/25") },
                new AccountProfile { AccountProfileId = 2, Gender = 1, Phone = "0123456789", Fullname = "Manager", Address = "HCM", Images = null, CreatedAt = DateOnly.Parse("1/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new AccountProfile { AccountProfileId = 3, Gender = 2, Phone = "0123456789", Fullname = "Staff 1", Address = "HCM", Images = null, CreatedAt = DateOnly.Parse("5/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new AccountProfile { AccountProfileId = 6, Gender = 1, Phone = "0123456789", Fullname = "Staff 2", Address = "HCM", Images = null, CreatedAt = DateOnly.Parse("5/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new AccountProfile { AccountProfileId = 4, Gender = 2, Phone = "0123456789", Fullname = "Customer 1", Address = "HCM", Images = null, CreatedAt = DateOnly.Parse("4/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new AccountProfile { AccountProfileId = 8, Gender = 1, Phone = "0123456789", Fullname = "Customer 3", Address = "HCM", Images = null, CreatedAt = DateOnly.Parse("4/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new AccountProfile { AccountProfileId = 7, Gender = 1, Phone = "0123456789", Fullname = "Customer 4", Address = "HCM", Images = null, CreatedAt = DateOnly.Parse("4/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new AccountProfile { AccountProfileId = 5, Gender = 2, Phone = "0123456789", Fullname = "Customer 2", Address = "HCM", Images = null, CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") },
                new AccountProfile { AccountProfileId = 9, Gender = 1, Phone = "0123456789", Fullname = "Staff 3", Address = "HCM", Images = null, CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("5/1/25") }
            );
            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Vegetable" },
                new Category { CategoryId = 3, CategoryName = "IOT 1" },
                new Category { CategoryId = 4, CategoryName = "Divice 1" },
                new Category { CategoryId = 2, CategoryName = "Fruit" }
                );
            builder.Entity<Product>().HasData(
                new Product { ProductId = 1, ProductName = "Vegetable 1", Images = null, Price = 10000, StockQuantity = 10000, Description = "Rau Den", Status = 1, CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("2/1/25"), CategoryId = 1, CropId = 1 },
                new Product { ProductId = 2, ProductName = "Vegetable 2", Images = null, Price = 15000, StockQuantity = 10000, Description = "Salad", Status = 1, CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("2/1/25"), CategoryId = 1, CropId = 1 },
                new Product { ProductId = 3, ProductName = "Vegetable 3", Images = null, Price = 20000, StockQuantity = 10000, Description = "Dau Ha Lan", Status = 1, CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("2/1/25"), CategoryId = 1, CropId = 1 },
                new Product { ProductId = 4, ProductName = "Fruit 1", Images = null, Price = 35000, StockQuantity = 10000, Description = "Ca chua", Status = 1, CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("2/1/25"), CategoryId = 1, CropId = 2 },
                new Product { ProductId = 5, ProductName = "Fruit 2", Images = null, Price = 55000, StockQuantity = 10000, Description = "Dua Hau", Status = 1, CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("2/1/25"), CategoryId = 1, CropId = 3 }
                );
            builder.Entity<Schedule>().HasData(
                new Schedule { ScheduleId = 1, StartDate = DateOnly.Parse("3/1/25"), EndDate = DateOnly.Parse("4/30/25"), Status = 1, CreatedAt = DateOnly.Parse("3/1/25"), UpdatedAt = DateOnly.Parse("3/1/25"), AssignedTo = 3, FarmActivityId = 1, FarmDetailsId = 1 },
                new Schedule { ScheduleId = 2, StartDate = DateOnly.Parse("3/12/25"), EndDate = DateOnly.Parse("4/11/25"), Status = 1, CreatedAt = DateOnly.Parse("3/1/25"), UpdatedAt = DateOnly.Parse("3/1/25"), AssignedTo = 9, FarmActivityId = 2, FarmDetailsId = 2 },
                new Schedule { ScheduleId = 3, StartDate = DateOnly.Parse("3/12/25"), EndDate = DateOnly.Parse("4/30/25"), Status = 1, CreatedAt = DateOnly.Parse("3/1/25"), UpdatedAt = DateOnly.Parse("3/1/25"), AssignedTo = 6, FarmActivityId = 3, FarmDetailsId = 3 }
           );
            builder.Entity<FarmActivity>().HasData(
                new FarmActivity { FarmActivitiesId = 1, ActivityType = 1, StartDate = DateOnly.Parse("3/1/25"), EndDate = DateOnly.Parse("4/1/25"), Status = 1 },
                new FarmActivity { FarmActivitiesId = 2, ActivityType = 1, StartDate = DateOnly.Parse("3/12/25"), EndDate = DateOnly.Parse("4/11/25"), Status = 1 },
                new FarmActivity { FarmActivitiesId = 3, ActivityType = 1, StartDate = DateOnly.Parse("3/12/25"), EndDate = DateOnly.Parse("4/30/25"), Status = 1 }
                );
            builder.Entity<FarmDetail>().HasData(
                new FarmDetail { FarmDetailsId = 1, FarmName = "Happy Farm", Location = "Q1, HCM", CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("3/1/25"), AccountId = 4 },
                new FarmDetail { FarmDetailsId = 2, FarmName = "Sky Garden", Location = "Thu Duc, HCM", CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("3/2/25"), AccountId = 5 },
                new FarmDetail { FarmDetailsId = 3, FarmName = "Hidden Planting", Location = "Binh Tan, HCM", CreatedAt = DateOnly.Parse("2/1/25"), UpdatedAt = DateOnly.Parse("3/5/25"), AccountId = 7 }
        );
            builder.Entity<Crop>().HasData(
                new Crop { CropId = 1, CropName = "Cop 01", Description = "3x3 m2", Status = 1, PlantingDate = DateOnly.Parse("3/1/25"), HarvestDate = DateOnly.Parse("4/21/25"), Moisture = 1, Temperature = 30, Fertilizer = "NPK", FarmDetailsId = 1, },
                new Crop { CropId = 2, CropName = "Cop 02", Description = "2x3 m2", Status = 1, PlantingDate = DateOnly.Parse("3/14/25"), HarvestDate = DateOnly.Parse("4/21/25"), Moisture = 1, Temperature = 30, Fertilizer = "Cali", FarmDetailsId = 1, },
                new Crop { CropId = 3, CropName = "Cop 03", Description = "3x2 m2", Status = 1, PlantingDate = DateOnly.Parse("3/21/25"), HarvestDate = DateOnly.Parse("4/11/25"), Moisture = 1, Temperature = 30, Fertilizer = "Photpho", FarmDetailsId = 1, },
                new Crop { CropId = 4, CropName = "Cop 04", Description = "3x5 m2", Status = 1, PlantingDate = DateOnly.Parse("3/12/25"), HarvestDate = DateOnly.Parse("4/30/25"), Moisture = 1, Temperature = 30, Fertilizer = "Nito", FarmDetailsId = 2, },
                new Crop { CropId = 5, CropName = "Cop 05", Description = "4x3 m2", Status = 1, PlantingDate = DateOnly.Parse("3/12/25"), HarvestDate = DateOnly.Parse("4/30/25"), Moisture = 1, Temperature = 30, Fertilizer = "NPK", FarmDetailsId = 3, }
                );
            builder.Entity<Feedback>().HasData(
                new Feedback { FeedbackId = 1, CustomerId = 4, CreatedAt = DateOnly.Parse("4/3/25"), Rating = 4, Comment = "App very good, but load data slow" },
                new Feedback { FeedbackId = 2, CustomerId = 7, CreatedAt = DateOnly.Parse("4/10/25"), Rating = 5, Comment = "My avt so cute <3" },
                new Feedback { FeedbackId = 3, CustomerId = 8, CreatedAt = DateOnly.Parse("3/3/25"), Rating = 5, Comment = "Test FeedBack <3" }
                );
            builder.Entity<IoTdevice>().HasData(
                new IoTdevice { IoTdevicesId = 1, DeviceName = "Thermocouple - 1", DeviceType = "Temperature IC", Status = 1, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("3/12/25"), ExpiryDate = DateOnly.Parse("4/30/25"), FarmDetailsId = 2 },
                new IoTdevice { IoTdevicesId = 2, DeviceName = "LM393 - 1", DeviceType = "Humidity measurement IC", Status = 1, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("3/12/25"), ExpiryDate = DateOnly.Parse("4/30/25"), FarmDetailsId = 1 },
                new IoTdevice { IoTdevicesId = 3, DeviceName = "LM393 - 2", DeviceType = "Humidity measurement IC", Status = 1, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("3/12/25"), ExpiryDate = DateOnly.Parse("4/30/25"), FarmDetailsId = 2 },
                new IoTdevice { IoTdevicesId = 4, DeviceName = "Thermocouple  - 2", DeviceType = "Temperature IC", Status = 1, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("3/12/25"), ExpiryDate = DateOnly.Parse("4/30/25"), FarmDetailsId = 3 },
                new IoTdevice { IoTdevicesId = 5, DeviceName = "Thermocouple  - 3", DeviceType = "Temperature IC", Status = 1, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("3/12/25"), ExpiryDate = DateOnly.Parse("4/30/25"), FarmDetailsId = 1 },
                new IoTdevice { IoTdevicesId = 6, DeviceName = "LM393 - 3", DeviceType = "Humidity measurement IC", Status = 1, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("3/12/25"), ExpiryDate = DateOnly.Parse("4/30/25"), FarmDetailsId = 3 },
                new IoTdevice { IoTdevicesId = 7, DeviceName = "Soil Moisture Sensor 1", DeviceType = "Soil Moisture Sensor IC", Status = 1, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("3/12/25"), ExpiryDate = DateOnly.Parse("4/30/25"), FarmDetailsId = 1 },
                new IoTdevice { IoTdevicesId = 8, DeviceName = "Soil Moisture Sensor 2", DeviceType = "Soil Moisture Sensor IC", Status = 1, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("3/12/25"), ExpiryDate = DateOnly.Parse("4/30/25"), FarmDetailsId = 2 },
                new IoTdevice { IoTdevicesId = 9, DeviceName = "Soil Moisture Sensor 3", DeviceType = "Soil Moisture Sensor IC", Status = 1, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("3/12/25"), ExpiryDate = DateOnly.Parse("4/30/25"), FarmDetailsId = 3 }
                );
            builder.Entity<Order>().HasData(
                new Order { OrderId = 1, CustomerId = 8, TotalPrice = 200, ShippingAddress = "HCM", Status = 1, CreatedAt = DateOnly.Parse("3/1/25"), UpdatedAt = DateOnly.Parse("3/1/25") },
                new Order { OrderId = 2, CustomerId = 8, TotalPrice = 105, ShippingAddress = "HCM", Status = 0, CreatedAt = DateOnly.Parse("3/1/25"), UpdatedAt = DateOnly.Parse("3/1/25") },
                new Order { OrderId = 3, CustomerId = 8, TotalPrice = 315, ShippingAddress = "HCM", Status = 1, CreatedAt = DateOnly.Parse("3/1/25"), UpdatedAt = DateOnly.Parse("3/1/25") }
                );
            builder.Entity<OrderDetail>().HasData(
                new OrderDetail { OrderDetailId = 1, OrderId = 1, Quantity = 20, UnitPrice = 10, ProductId = 1 },
                new OrderDetail { OrderDetailId = 2, OrderId = 1, Quantity = 10, UnitPrice = 15, ProductId = 2 },
                new OrderDetail { OrderDetailId = 3, OrderId = 1, Quantity = 21, UnitPrice = 10, ProductId = 2 }
                );
            builder.Entity<Payment>().HasData(
                new Payment { PaymentId = 1, OrderId = 1, PaymentStatus = 1, TransactionDate = DateOnly.Parse("3/1/25") },
                new Payment { PaymentId = 2, OrderId = 2, PaymentStatus = 0, TransactionDate = DateOnly.Parse("3/1/25") },
                new Payment { PaymentId = 3, OrderId = 3, PaymentStatus = 1, TransactionDate = DateOnly.Parse("3/1/25") }
                );
            builder.Entity<Report>().HasData(
                new Report { ReportId = 1, ReportDate = DateOnly.Parse("3/1/25"), ReportType = 1, FilePath = null, Status = 1, GeneratedBy = 2 },
                new Report { ReportId = 2, ReportDate = DateOnly.Parse("3/3/25"), ReportType = 3, FilePath = null, Status = 1, GeneratedBy = 2 },
                new Report { ReportId = 3, ReportDate = DateOnly.Parse("3/5/25"), ReportType = 2, FilePath = null, Status = 0, GeneratedBy = 2 },
                new Report { ReportId = 4, ReportDate = DateOnly.Parse("3/10/25"), ReportType = 1, FilePath = null, Status = 0, GeneratedBy = 2 },
                new Report { ReportId = 5, ReportDate = DateOnly.Parse("3/11/25"), ReportType = 3, FilePath = null, Status = 1, GeneratedBy = 2 }
                );


            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("Default");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

    }
}
