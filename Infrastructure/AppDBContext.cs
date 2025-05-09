using System.Reflection;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

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

        public DbSet<Crop> Crops { get; set; }

        public DbSet<CropRequirement> CropRequirement { get; set; }

        public DbSet<DailyLog> DailyLog { get; set; }

        public DbSet<Farm> Farm { get; set; }

        public DbSet<FarmActivity> FarmActivitie { get; set; }

        public DbSet<Feedback> Feedback { get; set; }

        public DbSet<Inventory> Inventorie { get; set; }

        public DbSet<IoTdevice> IoTdevice { get; set; }

        public DbSet<IoTdeviceLog> IoTdeviceLog { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderDetail> OrderDetail { get; set; }

        public DbSet<Payment> Payment { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<Schedule> Schedule { get; set; }
        public string rau3 = "https://www.hasfarmgreens.com/wp-content/uploads/2023/03/DSC_0255-Edit.jpg";
        public string rau2 = "https://www.vinmec.com/static/uploads/small_20201226_005345_144787_rau_den_max_1800x1800_jpg_aaca13f0a2.jpg";
        public string rau1 = "https://i1-vnexpress.vnecdn.net/2022/12/02/61lN5mpZAGL-SL1200-1-3093-1669931323.jpg?w=680&h=0&q=100&dpr=1&fit=crop&s=Cj5kP5ZFslHpx0ogALNRdA";
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            #region seedingdata
            builder.Entity<Account>().HasData(
                new Account { AccountId = 1, Email = "admin@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.Status.ACTIVE, Role = Domain.Enum.Roles.Admin, RefreshToken = null, ExpireMinute = 30, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") },
                new Account { AccountId = 2, Email = "manager@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.Status.ACTIVE, Role = Domain.Enum.Roles.Manager, RefreshToken = null, ExpireMinute = 30, CreatedAt = DateOnly.Parse("1/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 3, Email = "staff01@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.Status.ACTIVE, Role = Domain.Enum.Roles.Staff, RefreshToken = null, ExpireMinute = 30, CreatedAt = DateOnly.Parse("5/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 6, Email = "staff02@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.Status.ACTIVE, Role = Domain.Enum.Roles.Staff, RefreshToken = null, ExpireMinute = 30, CreatedAt = DateOnly.Parse("5/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 9, Email = "staff03@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.Status.ACTIVE, Role = Domain.Enum.Roles.Staff, RefreshToken = null, ExpireMinute = 30, CreatedAt = DateOnly.Parse("5/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 4, Email = "cus01@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.Status.ACTIVE, Role = Domain.Enum.Roles.Customer, RefreshToken = null, ExpireMinute = 30, CreatedAt = DateOnly.Parse("4/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 8, Email = "cus03@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.Status.ACTIVE, Role = Domain.Enum.Roles.Customer, RefreshToken = null, ExpireMinute = 30, CreatedAt = DateOnly.Parse("4/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 7, Email = "cus04@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.Status.ACTIVE, Role = Domain.Enum.Roles.Customer, RefreshToken = null, ExpireMinute = 30, CreatedAt = DateOnly.Parse("4/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 5, Email = "cus02@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.Status.ACTIVE, Role = Domain.Enum.Roles.Customer, RefreshToken = null, ExpireMinute = 30, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") }
            );
            builder.Entity<AccountProfile>().HasData(
                new AccountProfile { AccountProfileId = 1, Gender = Domain.Enum.Gender.Male, Phone = "0123456789", Fullname = "Admin", Address = "TP. Ho Chi Minh", Images = null, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") },
                new AccountProfile { AccountProfileId = 2, Gender = Domain.Enum.Gender.Male, Phone = "0123456789", Fullname = "Manager", Address = "TP. Ho Chi Minh", Images = null, CreatedAt = DateOnly.Parse("1/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new AccountProfile { AccountProfileId = 3, Gender = Domain.Enum.Gender.Female, Phone = "0123456789", Fullname = "Staff 1", Address = "TP. Ho Chi Minh", Images = null, CreatedAt = DateOnly.Parse("5/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new AccountProfile { AccountProfileId = 6, Gender = Domain.Enum.Gender.Male, Phone = "0123456789", Fullname = "Staff 2", Address = "TP. Ho Chi Minh", Images = null, CreatedAt = DateOnly.Parse("5/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new AccountProfile { AccountProfileId = 4, Gender = Domain.Enum.Gender.Female, Phone = "0123456789", Fullname = "Customer 1", Address = "TP. Ho Chi Minh", Images = null, CreatedAt = DateOnly.Parse("4/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new AccountProfile { AccountProfileId = 8, Gender = Domain.Enum.Gender.Male, Phone = "0123456789", Fullname = "Customer 3", Address = "TP. Ho Chi Minh", Images = null, CreatedAt = DateOnly.Parse("4/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new AccountProfile { AccountProfileId = 7, Gender = Domain.Enum.Gender.Male, Phone = "0123456789", Fullname = "Customer 4", Address = "TP. Ho Chi Minh", Images = null, CreatedAt = DateOnly.Parse("4/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new AccountProfile { AccountProfileId = 5, Gender = Domain.Enum.Gender.Female, Phone = "0123456789", Fullname = "Customer 2", Address = "TP. Ho Chi Minh", Images = null, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new AccountProfile { AccountProfileId = 9, Gender = Domain.Enum.Gender.Male, Phone = "0123456789", Fullname = "Staff 3", Address = "TP. Ho Chi Minh", Images = null, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") }
            );
            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Leafy Vegetables - Rau An La" },
                new Category { CategoryId = 2, CategoryName = "Root Vegetables - Rau Cu" },
                new Category { CategoryId = 3, CategoryName = "Fruiting Vegetables - Rau Qua" },
                new Category { CategoryId = 4, CategoryName = "Herbs and Spices - Rau Gia Vi" }
                );
            builder.Entity<Inventory>().HasData(
                new Inventory { InventoryId = 1, Location = "TP. Ho Chi Minh", StockQuantity = 1000, Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/1/2025"), ExpiryDate = DateOnly.Parse("6/1/2025"), ProductId = 1 },
                new Inventory { InventoryId = 2, Location = "TP. Ho Chi Minh", StockQuantity = 500, Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/11/2025"), ExpiryDate = DateOnly.Parse("6/1/2025"), ProductId = 2 },
                new Inventory { InventoryId = 3, Location = "TP. Ho Chi Minh", StockQuantity = 666, Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/8/2025"), ExpiryDate = DateOnly.Parse("6/1/2025"), ProductId = 3 }
                 );
            
            builder.Entity<Product>().HasData(
                new Product { ProductId = 1, ProductName = "Morning Glory - Rau Muong", Images = rau1, Price = 10000, StockQuantity = 10000, Description = "Rau Muong", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("2/1/2025"), CategoryId = 1, },
                new Product { ProductId = 2, ProductName = "Amarant - Rau Den", Images = rau2, Price = 15000, StockQuantity = 10000, Description = "Rau Den", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("2/1/2025"), CategoryId = 1, },
                new Product { ProductId = 3, ProductName = "Turnip - Cai", Images = rau3, Price = 20000, StockQuantity = 10000, Description = "Cai", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("2/1/2025"), CategoryId = 1, }
                //new Product { ProductId = 4, ProductName = "Fruit 1", Images = null, Price = 35000, StockQuantity = 10000, Description = "Ca chua", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("2/1/2025"), CategoryId = 1, },
                //new Product { ProductId = 5, ProductName = "Fruit 2", Images = null, Price = 55000, StockQuantity = 10000, Description = "Dua Hau", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("2/1/2025"), CategoryId = 1, }
                );
            builder.Entity<Schedule>().HasData(
               new Schedule { ScheduleId = 1, StartDate = DateOnly.Parse("3/1/2025"), EndDate = DateOnly.Parse("4/3/2025"), Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025"), AssignedTo = 3, FarmActivityId = 1, FarmDetailsId = 1, CropId = 1 },
                new Schedule { ScheduleId = 2, StartDate = DateOnly.Parse("5/4/2025"), EndDate = DateOnly.Parse("4/11/2025"), Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025"), AssignedTo = 9, FarmActivityId = 2, FarmDetailsId = 2, CropId = 2 },
                new Schedule { ScheduleId = 3, StartDate = DateOnly.Parse("5/4/2025"), EndDate = DateOnly.Parse("4/3/2025"), Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025"), AssignedTo = 6, FarmActivityId = 3, FarmDetailsId = 3, CropId = 3}
           );
            builder.Entity<FarmActivity>().HasData(
                new FarmActivity { FarmActivitiesId = 1, ActivityType = Domain.Enum.ActivityType.Sowing, StartDate = DateOnly.Parse("3/1/2025"), EndDate = DateOnly.Parse("4/1/2025"), Status = Domain.Enum.Status.ACTIVE },
                new FarmActivity { FarmActivitiesId = 2, ActivityType = Domain.Enum.ActivityType.Harvesting, StartDate = DateOnly.Parse("5/4/2025"), EndDate = DateOnly.Parse("4/11/2025"), Status = Domain.Enum.Status.ACTIVE },
                new FarmActivity { FarmActivitiesId = 3, ActivityType = Domain.Enum.ActivityType.Fertilization, StartDate = DateOnly.Parse("5/4/2025"), EndDate = DateOnly.Parse("4/3/2025"), Status = Domain.Enum.Status.ACTIVE }
                );
            builder.Entity<Farm>().HasData(
                new Farm { FarmId = 1, FarmName = "Rau Muong ", Location = "TP. Ho Chi Minh", CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025"), AccountId = 4 },
                new Farm { FarmId = 2, FarmName = "Rau Den", Location = "TP. Ho Chi Minh", CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("3/2/2025"), AccountId = 5 },
                new Farm { FarmId = 3, FarmName = "Cai", Location = "TP. Ho Chi Minh", CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("3/5/2025"), AccountId = 7 }
        );
            builder.Entity<Crop>().HasData(
                new Crop { CropId = 1, CropName = "Cop 01", Description = "3x3 m2", Status = Domain.Enum.Status.ACTIVE, PlantingDate = DateOnly.Parse("3/1/2025"), HarvestDate = DateOnly.Parse("10/10/2025"), Quantity = 100 },
                new Crop { CropId = 2, CropName = "Cop 02", Description = "2x3 m2", Status = Domain.Enum.Status.ACTIVE, PlantingDate = DateOnly.Parse("3/4/2025"), HarvestDate = DateOnly.Parse("5/5/2025"), Quantity = 100 },
                new Crop { CropId = 3, CropName = "Cop 03", Description = "3x2 m2", Status = Domain.Enum.Status.ACTIVE, PlantingDate = DateOnly.Parse("4/4/2025"), HarvestDate = DateOnly.Parse("11/10/2025"), Quantity = 100 },
                new Crop { CropId = 4, CropName = "Cop 04", Description = "3x5 m2", Status = Domain.Enum.Status.ACTIVE, PlantingDate = DateOnly.Parse("3/5/2025"), HarvestDate = DateOnly.Parse("10/11/2025"), Quantity = 100 },
                new Crop { CropId = 5, CropName = "Cop 05", Description = "4x3 m2", Status = Domain.Enum.Status.ACTIVE, PlantingDate = DateOnly.Parse("3/5/2025"), HarvestDate = DateOnly.Parse("7/8/2025"), Quantity = 100 }
                );
            builder.Entity<Feedback>().HasData(
                new Feedback { FeedbackId = 1, CustomerId = 4, CreatedAt = DateOnly.Parse("4/3/2025"), Rating = 4, Comment = "App very good, but load data slow" },
                new Feedback { FeedbackId = 2, CustomerId = 7, CreatedAt = DateOnly.Parse("4/10/2025"), Rating = 5, Comment = "My avt so cute <3" },
                new Feedback { FeedbackId = 3, CustomerId = 8, CreatedAt = DateOnly.Parse("3/3/2025"), Rating = 5, Comment = "Test FeedBack <3" }
                );
            builder.Entity<IoTdevice>().HasData(
                new IoTdevice { IoTdevicesId = 1, DeviceName = "Thermocouple - 1", DeviceType = "Temperature IC", Status = Domain.Enum.Status.ACTIVE, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025"), FarmDetailsId = 2 },
                new IoTdevice { IoTdevicesId = 2, DeviceName = "LM393 - 1", DeviceType = "Humidity measurement IC", Status = Domain.Enum.Status.ACTIVE, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025"), FarmDetailsId = 1 },
                new IoTdevice { IoTdevicesId = 3, DeviceName = "LM393 - 2", DeviceType = "Humidity measurement IC", Status = Domain.Enum.Status.ACTIVE, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025"), FarmDetailsId = 2 },
                new IoTdevice { IoTdevicesId = 4, DeviceName = "Thermocouple  - 2", DeviceType = "Temperature IC", Status = Domain.Enum.Status.ACTIVE, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025"), FarmDetailsId = 3 },
                new IoTdevice { IoTdevicesId = 5, DeviceName = "Thermocouple  - 3", DeviceType = "Temperature IC", Status = Domain.Enum.Status.ACTIVE, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025"), FarmDetailsId = 1 },
                new IoTdevice { IoTdevicesId = 6, DeviceName = "LM393 - 3", DeviceType = "Humidity measurement IC", Status = Domain.Enum.Status.ACTIVE, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025"), FarmDetailsId = 3 },
                new IoTdevice { IoTdevicesId = 7, DeviceName = "Soil Moisture Sensor 1", DeviceType = "Soil Moisture Sensor IC", Status = Domain.Enum.Status.ACTIVE, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025"), FarmDetailsId = 1 },
                new IoTdevice { IoTdevicesId = 8, DeviceName = "Soil Moisture Sensor 2", DeviceType = "Soil Moisture Sensor IC", Status = Domain.Enum.Status.ACTIVE, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025"), FarmDetailsId = 2 },
                new IoTdevice { IoTdevicesId = 9, DeviceName = "Soil Moisture Sensor 3", DeviceType = "Soil Moisture Sensor IC", Status = Domain.Enum.Status.ACTIVE, SensorValue = null, Unit = null, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025"), FarmDetailsId = 3 }
                );
            builder.Entity<Order>().HasData(
                new Order { OrderId = 1, CustomerId = 8, TotalPrice = 200, ShippingAddress = "TP. Ho Chi Minh", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") },
                new Order { OrderId = 2, CustomerId = 8, TotalPrice = 105, ShippingAddress = "TP. Ho Chi Minh", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") },
                new Order { OrderId = 3, CustomerId = 8, TotalPrice = 315, ShippingAddress = "TP. Ho Chi Minh", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") }
                );
            builder.Entity<OrderDetail>().HasData(
                new OrderDetail { OrderDetailId = 1, OrderId = 1, Quantity = 20, UnitPrice = 10, ProductId = 1 },
                new OrderDetail { OrderDetailId = 2, OrderId = 1, Quantity = 10, UnitPrice = 15, ProductId = 2 },
                new OrderDetail { OrderDetailId = 3, OrderId = 1, Quantity = 21, UnitPrice = 10, ProductId = 2 }
                );
            builder.Entity<Payment>().HasData(
                new Payment { PaymentId = 1, OrderId = 1, TransactionId = "VNPay01", Amount = 10, PaymentMethod = "VNPay", VnPayResponseCode = "VNPayPayment01", Success = true, PaymentTime = DateTime.Parse("3/1/2025"), CreateDate = DateTime.Parse("3/1/2025"), UpdateDate = null },
                new Payment { PaymentId = 2, OrderId = 2, TransactionId = "VNPay02", Amount = 15, PaymentMethod = "VNPay", VnPayResponseCode = "VNPayPayment02", Success = false, PaymentTime = DateTime.Parse("3/1/2025"), CreateDate = DateTime.Parse("3/1/2025"), UpdateDate = null },
                new Payment { PaymentId = 3, OrderId = 3, TransactionId = "VNPay03", Amount = 21, PaymentMethod = "VNPay", VnPayResponseCode = "VNPayPayment03", Success = false, PaymentTime = DateTime.Parse("3/1/2025"), CreateDate = DateTime.Parse("3/1/2025"), UpdateDate = null }
                );

            builder.Entity<CropRequirement>().HasData(
                new CropRequirement { RequirementId = 1, EstimatedDate = "60", Moisture = 1, Temperature = 29, Fertilizer = "NPK", DeviceId = 1 },
                new CropRequirement { RequirementId = 2, EstimatedDate = "45", Moisture = 1, Temperature = 22, Fertilizer = "NPK", DeviceId = 3 },
                new CropRequirement { RequirementId = 3, EstimatedDate = "30", Moisture = 1, Temperature = 26, Fertilizer = "NPK", DeviceId = 2 }
                );

            builder.Entity<IoTdeviceLog>().HasData(
                new IoTdeviceLog { LogId = 1, CreatedAt = DateOnly.Parse("4/5/2025"), IoTdevice = 1, TrackingId = 1 },
                new IoTdeviceLog { LogId = 2, CreatedAt = DateOnly.Parse("4/6/2025"), IoTdevice = 2, TrackingId = 2 },
                new IoTdeviceLog { LogId = 4, CreatedAt = DateOnly.Parse("4/6/2025"), IoTdevice = 2, TrackingId = 4 },
                new IoTdeviceLog { LogId = 5, CreatedAt = DateOnly.Parse("4/6/2025"), IoTdevice = 2, TrackingId = 5 },
                new IoTdeviceLog { LogId = 6, CreatedAt = DateOnly.Parse("4/6/2025"), IoTdevice = 2, TrackingId = 6 },
                new IoTdeviceLog { LogId = 3, CreatedAt = DateOnly.Parse("4/7/2025"), IoTdevice = 3, TrackingId = 3 }
                );
            builder.Entity<DailyLog>().HasData(
                 new DailyLog { TrackingId = 1, Date = DateOnly.Parse("4/5/2025"), Notes = "Hoat dong binh thuong", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("4/5/2025"), UpdatedAt = DateOnly.Parse("4/5/2025"), AssignedTo = 3, ScheduleId = 1 },
                 new DailyLog { TrackingId = 2, Date = DateOnly.Parse("4/6/2025"), Notes = "Hoat dong binh thuong", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("4/6/2025"), UpdatedAt = DateOnly.Parse("4/6/2025"), AssignedTo = 6, ScheduleId = 2 },
                 new DailyLog { TrackingId = 4, Date = DateOnly.Parse("4/7/2025"), Notes = "Hoat dong binh thuong", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("4/7/2025"), UpdatedAt = DateOnly.Parse("4/7/2025"), AssignedTo = 6, ScheduleId = 2 },
                 new DailyLog { TrackingId = 5, Date = DateOnly.Parse("4/8/2025"), Notes = "Hoat dong binh thuong", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("4/8/2025"), UpdatedAt = DateOnly.Parse("4/8/2025"), AssignedTo = 6, ScheduleId = 2 },
                 new DailyLog { TrackingId = 6, Date = DateOnly.Parse("4/9/2025"), Notes = "Hoat dong binh thuong", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("4/9/2025"), UpdatedAt = DateOnly.Parse("4/9/2025"), AssignedTo = 6, ScheduleId = 2 },
                 new DailyLog { TrackingId = 3, Date = DateOnly.Parse("4/7/2025"), Notes = "Hoat dong binh thuong", Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("4/7/2025"), UpdatedAt = DateOnly.Parse("4/7/2025"), AssignedTo = 9, ScheduleId = 3 }

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
