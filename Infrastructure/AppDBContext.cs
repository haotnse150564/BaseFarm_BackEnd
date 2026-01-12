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
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        //{
        //    optionBuilder.UseSqlServer("ConnectionsString");
        //}
        public DbSet<Account> Account { get; set; }

        public DbSet<AccountProfile> AccountProfile { get; set; }

        public DbSet<Category> Categorie { get; set; }

        public DbSet<Crop> Crops { get; set; }

        public DbSet<CropRequirement> CropRequirement { get; set; }

        public DbSet<IOTLog> IOTLog { get; set; }

        public DbSet<Farm> Farm { get; set; }

        public DbSet<FarmActivity> FarmActivity { get; set; }
        public DbSet<FarmEquipment> FarmEquipment { get; set; }

        public DbSet<Feedback> Feedback { get; set; }

        public DbSet<Inventory> Inventorie { get; set; }

        public DbSet<Device> Devices { get; set; }

        //public DbSet<DevicesLog> DevicesLog { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderDetail> OrderDetail { get; set; }

        public DbSet<Payment> Payment { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<ScheduleLog> ScheduleLog { get; set; }
        public DbSet<Staff_FarmActivity> StaffFarmActivities { get; set; }

        public string rau3 = "https://cdn.tgdd.vn/2021/08/CookProduct/dau-do-52-1200x676.jpg";
        public string rau2 = "https://www.vinmec.com/static/uploads/small_20201226_005345_144787_rau_den_max_1800x1800_jpg_aaca13f0a2.jpg";
        public string rau1 = "https://i1-vnexpress.vnecdn.net/2022/12/02/61lN5mpZAGL-SL1200-1-3093-1669931323.jpg?w=680&h=0&q=100&dpr=1&fit=crop&s=Cj5kP5ZFslHpx0ogALNRdA";
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            #region seedingdata
            builder.Entity<Account>().HasData(
                new Account { AccountId = 1, Email = "admin@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.AccountStatus.ACTIVE, Role = Domain.Enum.Roles.Admin,/* RefreshToken = null, ExpireMinute = 30,*/ CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") },
                new Account { AccountId = 2, Email = "manager@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.AccountStatus.ACTIVE, Role = Domain.Enum.Roles.Manager, CreatedAt = DateOnly.Parse("1/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 3, Email = "staff01@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.AccountStatus.ACTIVE, Role = Domain.Enum.Roles.Staff, CreatedAt = DateOnly.Parse("5/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 6, Email = "staff02@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.AccountStatus.ACTIVE, Role = Domain.Enum.Roles.Staff, CreatedAt = DateOnly.Parse("5/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 9, Email = "staff03@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.AccountStatus.ACTIVE, Role = Domain.Enum.Roles.Staff, CreatedAt = DateOnly.Parse("5/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 4, Email = "cus01@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.AccountStatus.ACTIVE, Role = Domain.Enum.Roles.Customer, CreatedAt = DateOnly.Parse("4/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 8, Email = "cus03@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.AccountStatus.ACTIVE, Role = Domain.Enum.Roles.Customer, CreatedAt = DateOnly.Parse("4/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 7, Email = "cus04@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.AccountStatus.ACTIVE, Role = Domain.Enum.Roles.Customer, CreatedAt = DateOnly.Parse("4/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") },
                new Account { AccountId = 5, Email = "cus02@email.com", PasswordHash = "$2a$11$qrla7EUbH1Npf1e/ei6MlOrUcxeogYG/HdGAsr937SE9RWQp8Zwuq", Status = Domain.Enum.AccountStatus.ACTIVE, Role = Domain.Enum.Roles.Customer, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("5/1/2025") }
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
                new Category { CategoryId = 1, CategoryName = "Rau Ăn Lá" },
                new Category { CategoryId = 2, CategoryName = "Rau Củ" },
                new Category { CategoryId = 3, CategoryName = "Rau Gia Vị" }
                );
            builder.Entity<Inventory>().HasData(
                new Inventory { InventoryId = 1, Location = "TP. Ho Chi Minh", StockQuantity = 1000, Status = Domain.Enum.Status.ACTIVE, CreateAt = DateOnly.Parse("3/1/2025"), ExpireDate = DateOnly.Parse("6/1/2025"), ProductId = 1, ScheduleId = 1 },
                new Inventory { InventoryId = 2, Location = "TP. Ho Chi Minh", StockQuantity = 500, Status = Domain.Enum.Status.ACTIVE, CreateAt = DateOnly.Parse("3/11/2025"), ExpireDate = DateOnly.Parse("6/1/2025"), ProductId = 2, ScheduleId = 2 },
                new Inventory { InventoryId = 3, Location = "TP. Ho Chi Minh", StockQuantity = 666, Status = Domain.Enum.Status.ACTIVE, CreateAt = DateOnly.Parse("3/8/2025"), ExpireDate = DateOnly.Parse("6/1/2025"), ProductId = 3, ScheduleId = 3 }
                 );

            builder.Entity<Product>().HasData(
                new Product { ProductId = 1, ProductName = "Rau Muống ", Images = rau1, Price = 10000, StockQuantity = 10000, Description = "Rau Muống Sản Phẩm Thử Nghiệm", Status = Domain.Enum.ProductStatus.ACTIVE, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("2/1/2025"), CategoryId = 1, },
                new Product { ProductId = 2, ProductName = "Rau Dền", Images = rau1, Price = 15000, StockQuantity = 10000, Description = "Rau Dền Sản Phẩm Thử Nghiệm", Status = Domain.Enum.ProductStatus.ACTIVE, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("2/1/2025"), CategoryId = 1, },
                new Product { ProductId = 3, ProductName = "Rau Chân Vịt", Images = rau3, Price = 20000, StockQuantity = 10000, Description = "Rau Chân Vịt Sản Phẩm thử Nghiệm", Status = Domain.Enum.ProductStatus.ACTIVE, CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("2/1/2025"), CategoryId = 1, }
             );
            builder.Entity<Schedule>().HasData(
               new Schedule { ScheduleId = 1, StartDate = DateOnly.Parse("3/1/2025"), EndDate = DateOnly.Parse("4/3/2025"), Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025"),ManagerId = 1, FarmId = 1, CropId = 1, Quantity = 100 },
                new Schedule { ScheduleId = 2, StartDate = DateOnly.Parse("5/4/2025"), EndDate = DateOnly.Parse("4/11/2025"), Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025"), ManagerId = 1, FarmId = 1, CropId = 2, Quantity = 100 },
                new Schedule { ScheduleId = 3, StartDate = DateOnly.Parse("5/4/2025"), EndDate = DateOnly.Parse("4/3/2025"), Status = Domain.Enum.Status.ACTIVE, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025"), ManagerId = 1, FarmId = 1, CropId = 3, Quantity = 100 }
           );
            
            builder.Entity<Farm>().HasData(
                new Farm { FarmId = 1, FarmName = "Vườn Rau Sài Gòn", Location = "TP. Ho Chi Minh", CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025"), AccountId = 4, }
        //  new Farm { FarmId = 2, FarmName = "Rau Den", Location = "TP. Ho Chi Minh", CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("3/2/2025"), AccountId = 5 },
        //  new Farm { FarmId = 3, FarmName = "Cai", Location = "TP. Ho Chi Minh", CreatedAt = DateOnly.Parse("2/1/2025"), UpdatedAt = DateOnly.Parse("3/5/2025"), AccountId = 7 }
        );
            builder.Entity<Crop>().HasData(
                new Crop { CropId = 1, CropName = "Cop 01", Description = "3x3 m2", Status = Domain.Enum.CropStatus.ACTIVE, CategoryId = 1, CreateAt = DateOnly.Parse("3/1/2025"), UpdateAt = DateOnly.Parse("3/1/2025") },
                new Crop { CropId = 2, CropName = "Cop 02", Description = "2x3 m2", Status = Domain.Enum.CropStatus.ACTIVE, CategoryId = 1, CreateAt = DateOnly.Parse("3/1/2025"), UpdateAt = DateOnly.Parse("3/1/2025") },
                //new Crop { CropId = 3, CropName = "Cop 03", Description = "3x2 m2", Status = Domain.Enum.CropStatus.ACTIVE, CategoryId = 1, CreateAt = DateOnly.Parse("3/1/2025"), UpdateAt = DateOnly.Parse("3/1/2025") },
                //new Crop { CropId = 4, CropName = "Cop 04", Description = "3x5 m2", Status = Domain.Enum.CropStatus.ACTIVE, CategoryId = 3, CreateAt = DateOnly.Parse("3/1/2025"), UpdateAt = DateOnly.Parse("3/1/2025") },
                new Crop { CropId = 3, CropName = "Cop 03", Description = "4x3 m2", Status = Domain.Enum.CropStatus.ACTIVE, CategoryId = 3, CreateAt = DateOnly.Parse("3/1/2025"), UpdateAt = DateOnly.Parse("3/1/2025") }
                );

            builder.Entity<Device>().HasData(
                new Device { DevicesId = 1, Pin="V996", DeviceName = "Thermocouple - 1", DeviceType = "Temperature IC", Status = Domain.Enum.Status.ACTIVE, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025")},
                new Device { DevicesId = 2, Pin = "V997", DeviceName = "LM393 - 1", DeviceType = "Humidity measurement IC", Status = Domain.Enum.Status.ACTIVE, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025")},
                new Device { DevicesId = 3, Pin = "V998", DeviceName = "Soil Moisture Sensor 1", DeviceType = "Soil Moisture Sensor IC", Status = Domain.Enum.Status.ACTIVE, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025")},
                new Device { DevicesId = 4, Pin = "V999", DeviceName = "Led Light", DeviceType = "Soil Moisture Sensor IC", Status = Domain.Enum.Status.ACTIVE, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025")}
                //  new Device { DevicesId = 9, DeviceName = "Soil Moisture Sensor 3", DeviceType = "Soil Moisture Sensor IC", Status = Domain.Enum.Status.ACTIVE, LastUpdate = DateOnly.Parse("5/4/2025"), ExpiryDate = DateOnly.Parse("4/3/2025"), FarmDetailsId = 3 }
                );
            builder.Entity<Order>().HasData(
                new Order { OrderId = 1, CustomerId = 8, TotalPrice = 200000, ShippingAddress = "TP. Ho Chi Minh", Status = Domain.Enum.PaymentStatus.PAID, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") },
                new Order { OrderId = 2, CustomerId = 8, TotalPrice = 105000, ShippingAddress = "TP. Ho Chi Minh", Status = Domain.Enum.PaymentStatus.PAID, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") },
                new Order { OrderId = 3, CustomerId = 8, TotalPrice = 315000, ShippingAddress = "TP. Ho Chi Minh", Status = Domain.Enum.PaymentStatus.PAID, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") }
                //new Order { OrderId = 4, CustomerId = 8, TotalPrice = 300000, ShippingAddress = "TP. Ho Chi Minh", Status = Domain.Enum.PaymentStatus.PAID, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") },
                //new Order { OrderId = 5, CustomerId = 8, TotalPrice = 150000, ShippingAddress = "TP. Ho Chi Minh", Status = Domain.Enum.PaymentStatus.PAID, CreatedAt = DateOnly.Parse("3/1/2025"), UpdatedAt = DateOnly.Parse("3/1/2025") }
                );
            builder.Entity<OrderDetail>().HasData(
                new OrderDetail { OrderDetailId = 1, OrderId = 1, Quantity = 20, UnitPrice = 10, ProductId = 1 },
                new OrderDetail { OrderDetailId = 2, OrderId = 2, Quantity = 10, UnitPrice = 15, ProductId = 2 },
                new OrderDetail { OrderDetailId = 3, OrderId = 3, Quantity = 21, UnitPrice = 10, ProductId = 2 }
            //    new OrderDetail { OrderDetailId = 4, OrderId = 3, Quantity = 20, UnitPrice = 10, ProductId = 2 },
            //    new OrderDetail { OrderDetailId = 5, OrderId = 3, Quantity = 10, UnitPrice = 10, ProductId = 2 }
                );
            //builder.Entity<Payment>().HasData(
            //    new Payment { PaymentId = 1, OrderId = 1, TransactionId = "VNPay01", Amount = 10, PaymentMethod = "VNPay", VnPayResponseCode = "VNPayPayment01", Success = true, PaymentTime = DateTime.Parse("3/1/2025").ToUniversalTime(), CreateDate = DateTime.Parse("3/1/2025").ToUniversalTime(), UpdateDate = null },
            //    new Payment { PaymentId = 2, OrderId = 2, TransactionId = "VNPay02", Amount = 15, PaymentMethod = "VNPay", VnPayResponseCode = "VNPayPayment02", Success = false, PaymentTime = DateTime.Parse("3/1/2025").ToUniversalTime(), CreateDate = DateTime.Parse("3/1/2025").ToUniversalTime(), UpdateDate = null },
            //    new Payment { PaymentId = 3, OrderId = 3, TransactionId = "VNPay03", Amount = 21, PaymentMethod = "VNPay", VnPayResponseCode = "VNPayPayment03", Success = false, PaymentTime = DateTime.Parse("3/1/2025").ToUniversalTime(), CreateDate = DateTime.Parse("3/1/2025").ToUniversalTime(), UpdateDate = null }
            //    );
            builder.Entity<FarmEquipment>().HasData(
                new FarmEquipment { FarmEquipmentId = 1, AssignDate = DateOnly.Parse("3/1/2025"), Status = Domain.Enum.Status.ACTIVE, FarmId = 1, DeviceId = 1 },
                new FarmEquipment { FarmEquipmentId = 2, AssignDate = DateOnly.Parse("3/1/2025"), Status = Domain.Enum.Status.ACTIVE, FarmId = 1, DeviceId = 2 },
                new FarmEquipment { FarmEquipmentId = 3, AssignDate = DateOnly.Parse("3/1/2025"), Status = Domain.Enum.Status.ACTIVE, FarmId = 1, DeviceId = 3 }
                );
            builder.Entity<CropRequirement>().HasData(
          new CropRequirement
          {
              CropRequirementId = 1,
              EstimatedDate = 25,
              SoilMoisture = 1,
              Temperature = 29,
              Humidity = 70,
              PlantStage = Domain.Enum.PlantStage.Seedling,
              LightRequirement = 200,
              WateringFrequency = 7,
              CreatedDate = DateOnly.Parse("2025-11-11"),
              Fertilizer = "NPK",
              CropId = 1
          },
          new CropRequirement
          {
              CropRequirementId = 2,
              EstimatedDate = 25,
              SoilMoisture = 1,
              Temperature = 24,
              Humidity = 60,
              PlantStage = Domain.Enum.PlantStage.Preparation,
              LightRequirement = 200,
              WateringFrequency = 7,
              CreatedDate = DateOnly.Parse("2025-11-11"),
              Fertilizer = "NPK",
              CropId = 2
          },
          new CropRequirement
          {
              CropRequirementId = 3,
              EstimatedDate = 30,
              SoilMoisture = 1,
              Humidity = 60,
              Temperature = 26,
              PlantStage = Domain.Enum.PlantStage.Harvest,
              LightRequirement = 200,
              WateringFrequency = 7,
              CreatedDate = DateOnly.Parse("2025-11-11"),
              Fertilizer = "NPK",
              CropId = 3
          }
      );

            builder.Entity<Feedback>().HasData(
                new Feedback { FeedbackId = 1, OrderDetailId = 1, CustomerId = 4, CreatedAt = DateOnly.Parse("4/3/2025"), Status = Domain.Enum.Status.ACTIVE, Rating = 4, Comment = "Shop giao hàng nhanh" },
                new Feedback { FeedbackId = 2, OrderDetailId = 2, CustomerId = 7, CreatedAt = DateOnly.Parse("4/10/2025"), Status = Domain.Enum.Status.ACTIVE, Rating = 5, Comment = "Rau ngon" },
                new Feedback { FeedbackId = 3, OrderDetailId = 3, CustomerId = 8, CreatedAt = DateOnly.Parse("3/3/2025"), Status = Domain.Enum.Status.ACTIVE, Rating = 5, Comment = "Rau chất lượng, rẻ" }
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
                var connectionString = configuration.GetConnectionString("Default"); //"Host=localhost;Port=5432;Database=IOTBaseFarm;Username=postgres;Password=123456789";
                optionsBuilder.UseNpgsql(connectionString);


            }
        }

    }
}
