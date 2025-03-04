using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model;

public partial class IotbaseFarmContext : DbContext
{
    public IotbaseFarmContext()
    {
    }

    public IotbaseFarmContext(DbContextOptions<IotbaseFarmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountProfile> AccountProfiles { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Crop> Crops { get; set; }

    public virtual DbSet<FarmActivity> FarmActivities { get; set; }

    public virtual DbSet<FarmDetail> FarmDetails { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<IoTdevice> IoTdevices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=123456789;Database=IOTBaseFarm;Trusted_Connection=true;Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__F267253E931DEAA1");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId).HasColumnName("accountID");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.ExpireMinutes).HasColumnName("expireMinutes");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("passwordHash");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("refreshToken");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
        });

        modelBuilder.Entity<AccountProfile>(entity =>
        {
            entity.HasKey(e => e.AccountProfileId).HasName("PK__AccountP__33BADE4FB37C7932");

            entity.ToTable("AccountProfile");

            entity.Property(e => e.AccountProfileId)
                .ValueGeneratedNever()
                .HasColumnName("accountProfileID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.Fullname)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Images)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("images");
            entity.Property(e => e.Phone)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            entity.HasOne(d => d.AccountProfileNavigation).WithOne(p => p.AccountProfile)
                .HasForeignKey<AccountProfile>(d => d.AccountProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKAccountPro821701");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__23CAF1F8ED7CD103");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("categoryID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("categoryName");
        });

        modelBuilder.Entity<Crop>(entity =>
        {
            entity.HasKey(e => e.CropId).HasName("PK__Crop__DEDED39A50499E80");

            entity.ToTable("Crop");

            entity.Property(e => e.CropId).HasColumnName("cropID");
            entity.Property(e => e.CropName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cropName");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.FarmDetailsId).HasColumnName("farmDetailsID");
            entity.Property(e => e.Fertilizer)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("fertilizer");
            entity.Property(e => e.HarvestDate).HasColumnName("harvestDate");
            entity.Property(e => e.Moisture)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("moisture");
            entity.Property(e => e.PlantingDate).HasColumnName("plantingDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Temperature)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("temperature");

            entity.HasOne(d => d.FarmDetails).WithMany(p => p.Crops)
                .HasForeignKey(d => d.FarmDetailsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKCrop118260");
        });

        modelBuilder.Entity<FarmActivity>(entity =>
        {
            entity.HasKey(e => e.FarmActivitiesId).HasName("PK__FarmActi__1DB052C11518A8F5");

            entity.Property(e => e.FarmActivitiesId).HasColumnName("farmActivitiesID");
            entity.Property(e => e.ActivityType).HasColumnName("activityType");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<FarmDetail>(entity =>
        {
            entity.HasKey(e => e.FarmDetailsId).HasName("PK__FarmDeta__79BBB17D835AD9C5");

            entity.ToTable("FarmDetail");

            entity.Property(e => e.FarmDetailsId).HasColumnName("farmDetailsID");
            entity.Property(e => e.AccountId).HasColumnName("accountID");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.FarmName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("farmName");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            entity.HasOne(d => d.Account).WithMany(p => p.FarmDetails)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKFarmDetail922812");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__2613FDC401B6AF7E");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedbackID");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("comment");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.CustomerId).HasColumnName("customerID");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKFeedback230994");
        });

        modelBuilder.Entity<IoTdevice>(entity =>
        {
            entity.HasKey(e => e.IoTdevicesId).HasName("PK__IoTDevic__E4F4D892A0DF5378");

            entity.ToTable("IoTDevice");

            entity.Property(e => e.IoTdevicesId).HasColumnName("ioTDevicesID");
            entity.Property(e => e.DeviceName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("deviceName");
            entity.Property(e => e.DeviceType)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("deviceType");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiryDate");
            entity.Property(e => e.FarmDetailsId).HasColumnName("farmDetailsID");
            entity.Property(e => e.LastUpdate).HasColumnName("lastUpdate");
            entity.Property(e => e.SensorValue)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("sensorValue");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Unit)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("unit");

            entity.HasOne(d => d.FarmDetails).WithMany(p => p.IoTdevices)
                .HasForeignKey(d => d.FarmDetailsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKIoTDevice 127722");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__0809337D04375B27");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("orderID");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.CustomerId).HasColumnName("customerID");
            entity.Property(e => e.ShippingAddress)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("shippingAddress");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("totalPrice");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrder542182");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__E4FEDE2A731886BE");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("orderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("orderID");
            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unitPrice");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrderDetai440548");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrderDetai727100");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__A0D9EFA6C3585CFB");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("paymentID");
            entity.Property(e => e.OrderId).HasColumnName("orderID");
            entity.Property(e => e.PaymentMethod).HasColumnName("paymentMethod");
            entity.Property(e => e.PaymentStatus).HasColumnName("paymentStatus");
            entity.Property(e => e.TransactionDate).HasColumnName("transactionDate");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPayment796685");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__2D10D14A9AE33F34");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.CategoryId).HasColumnName("categoryID");
            entity.Property(e => e.CropId).HasColumnName("cropID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Images)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("images");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("productName");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StockQuantity).HasColumnName("stockQuantity");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProduct570888");

            entity.HasOne(d => d.Crop).WithMany(p => p.Products)
                .HasForeignKey(d => d.CropId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProduct679405");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Report__1C9B4ECDC860DCC4");

            entity.ToTable("Report");

            entity.Property(e => e.ReportId).HasColumnName("reportID");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.GeneratedByNavigation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.GeneratedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKReport748588");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__A532EDB4C215505E");

            entity.ToTable("Schedule");

            entity.Property(e => e.ScheduleId).HasColumnName("scheduleID");
            entity.Property(e => e.AssignedTo).HasColumnName("assignedTo");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.FarmActivityId).HasColumnName("farmActivityID");
            entity.Property(e => e.FarmDetailsId).HasColumnName("farmDetailsID");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchedule950653");

            entity.HasOne(d => d.FarmActivity).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.FarmActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchedule760059");

            entity.HasOne(d => d.FarmDetails).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.FarmDetailsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKSchedule504744");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
