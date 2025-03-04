﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Model.Account", b =>
                {
                    b.Property<long>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AccountId"));

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ExpireMinutes")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Role")
                        .HasColumnType("int");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateOnly?>("UpdatedAt")
                        .HasColumnType("date");

                    b.HasKey("AccountId");

                    b.ToTable("Account", (string)null);
                });

            modelBuilder.Entity("Domain.Model.AccountProfile", b =>
                {
                    b.Property<long>("AccountProfileId")
                        .HasColumnType("bigint");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<string>("Fullname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("Images")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Phone")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<DateOnly?>("UpdatedAt")
                        .HasColumnType("date");

                    b.HasKey("AccountProfileId");

                    b.ToTable("AccountProfile", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Category", b =>
                {
                    b.Property<long>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("CategoryId"));

                    b.Property<string>("CategoryName")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.HasKey("CategoryId");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Crop", b =>
                {
                    b.Property<long>("CropId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("CropId"));

                    b.Property<string>("CropName")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<long>("FarmDetailsId")
                        .HasColumnType("bigint");

                    b.Property<string>("Fertilizer")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("fertilizer");

                    b.Property<DateOnly?>("HarvestDate")
                        .HasColumnType("date");

                    b.Property<decimal?>("Moisture")
                        .HasColumnType("decimal(5, 2)")
                        .HasColumnName("moisture");

                    b.Property<DateOnly?>("PlantingDate")
                        .HasColumnType("date");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<decimal?>("Temperature")
                        .HasColumnType("decimal(5, 2)");

                    b.HasKey("CropId");

                    b.HasIndex("FarmDetailsId");

                    b.ToTable("Crop", (string)null);
                });

            modelBuilder.Entity("Domain.Model.FarmActivity", b =>
                {
                    b.Property<int>("FarmActivitiesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FarmActivitiesId"));

                    b.Property<int?>("ActivityType")
                        .HasColumnType("int");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("StartDate")
                        .HasColumnType("date");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.HasKey("FarmActivitiesId");

                    b.ToTable("FarmActivity", (string)null);
                });

            modelBuilder.Entity("Domain.Model.FarmDetail", b =>
                {
                    b.Property<long>("FarmDetailsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("FarmDetailsId"));

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<string>("FarmName")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Location")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<DateOnly?>("UpdatedAt")
                        .HasColumnType("date");

                    b.HasKey("FarmDetailsId");

                    b.HasIndex("AccountId");

                    b.ToTable("FarmDetail", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Feedback", b =>
                {
                    b.Property<int>("FeedbackId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeedbackId"));

                    b.Property<string>("Comment")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<long>("CustomerId")
                        .HasColumnType("bigint");

                    b.Property<int?>("Rating")
                        .HasColumnType("int");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.HasKey("FeedbackId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Feedback", (string)null);
                });

            modelBuilder.Entity("Domain.Model.IoTdevice", b =>
                {
                    b.Property<long>("IoTdevicesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("IoTdevicesId"));

                    b.Property<string>("DeviceName")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("DeviceType")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<DateOnly?>("ExpiryDate")
                        .HasColumnType("date");

                    b.Property<long>("FarmDetailsId")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("LastUpdate")
                        .HasColumnType("date");

                    b.Property<string>("SensorValue")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Unit")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.HasKey("IoTdevicesId");

                    b.HasIndex("FarmDetailsId");

                    b.ToTable("IoTDevice", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Order", b =>
                {
                    b.Property<long>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("OrderId"));

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<long>("CustomerId")
                        .HasColumnType("bigint");

                    b.Property<string>("ShippingAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<decimal?>("TotalPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateOnly?>("UpdatedAt")
                        .HasColumnType("date");

                    b.HasKey("OrderId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Order", (string)null);
                });

            modelBuilder.Entity("Domain.Model.OrderDetail", b =>
                {
                    b.Property<long>("OrderDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("OrderDetailId"));

                    b.Property<long>("OrderId")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal?>("UnitPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("OrderDetailId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderDetail", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Payment", b =>
                {
                    b.Property<long>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("PaymentId"));

                    b.Property<long>("OrderId")
                        .HasColumnType("bigint");

                    b.Property<int?>("PaymentMethod")
                        .HasColumnType("int");

                    b.Property<int?>("PaymentStatus")
                        .HasColumnType("int");

                    b.Property<DateOnly?>("TransactionDate")
                        .HasColumnType("date");

                    b.HasKey("PaymentId");

                    b.HasIndex("OrderId");

                    b.ToTable("Payment", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Product", b =>
                {
                    b.Property<long>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ProductId"));

                    b.Property<long>("CategoryId")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<long>("CropId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Images")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<int?>("StockQuantity")
                        .HasColumnType("int");

                    b.Property<DateOnly?>("UpdatedAt")
                        .HasColumnType("date");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CropId");

                    b.ToTable("Product", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Report", b =>
                {
                    b.Property<long>("ReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ReportId"));

                    b.Property<string>("FilePath")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<long>("GeneratedBy")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("ReportDate")
                        .HasColumnType("date");

                    b.Property<int?>("ReportType")
                        .HasColumnType("int");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.HasKey("ReportId");

                    b.HasIndex("GeneratedBy");

                    b.ToTable("Report", (string)null);
                });

            modelBuilder.Entity("Domain.Model.Schedule", b =>
                {
                    b.Property<long>("ScheduleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ScheduleId"));

                    b.Property<long>("AssignedTo")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("date");

                    b.Property<int>("FarmActivityId")
                        .HasColumnType("int");

                    b.Property<long>("FarmDetailsId")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("StartDate")
                        .HasColumnType("date");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateOnly?>("UpdatedAt")
                        .HasColumnType("date");

                    b.HasKey("ScheduleId");

                    b.HasIndex("AssignedTo");

                    b.HasIndex("FarmActivityId");

                    b.HasIndex("FarmDetailsId");

                    b.ToTable("Schedule", (string)null);
                });

            modelBuilder.Entity("Domain.Model.AccountProfile", b =>
                {
                    b.HasOne("Domain.Model.Account", "AccountProfileNavigation")
                        .WithOne("AccountProfile")
                        .HasForeignKey("Domain.Model.AccountProfile", "AccountProfileId")
                        .IsRequired();

                    b.Navigation("AccountProfileNavigation");
                });

            modelBuilder.Entity("Domain.Model.Crop", b =>
                {
                    b.HasOne("Domain.Model.FarmDetail", "FarmDetails")
                        .WithMany("Crops")
                        .HasForeignKey("FarmDetailsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FarmDetails");
                });

            modelBuilder.Entity("Domain.Model.FarmDetail", b =>
                {
                    b.HasOne("Domain.Model.Account", "Account")
                        .WithMany("FarmDetails")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Domain.Model.Feedback", b =>
                {
                    b.HasOne("Domain.Model.Account", "Customer")
                        .WithMany("Feedbacks")
                        .HasForeignKey("CustomerId")
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Domain.Model.IoTdevice", b =>
                {
                    b.HasOne("Domain.Model.FarmDetail", "FarmDetails")
                        .WithMany("IoTdevices")
                        .HasForeignKey("FarmDetailsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FarmDetails");
                });

            modelBuilder.Entity("Domain.Model.Order", b =>
                {
                    b.HasOne("Domain.Model.Account", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Domain.Model.OrderDetail", b =>
                {
                    b.HasOne("Domain.Model.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .IsRequired();

                    b.HasOne("Domain.Model.Product", "Product")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ProductId")
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Domain.Model.Payment", b =>
                {
                    b.HasOne("Domain.Model.Order", "Order")
                        .WithMany("Payments")
                        .HasForeignKey("OrderId")
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Domain.Model.Product", b =>
                {
                    b.HasOne("Domain.Model.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .IsRequired();

                    b.HasOne("Domain.Model.Crop", "Crop")
                        .WithMany("Products")
                        .HasForeignKey("CropId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Crop");
                });

            modelBuilder.Entity("Domain.Model.Report", b =>
                {
                    b.HasOne("Domain.Model.Account", "GeneratedByNavigation")
                        .WithMany("Reports")
                        .HasForeignKey("GeneratedBy")
                        .IsRequired();

                    b.Navigation("GeneratedByNavigation");
                });

            modelBuilder.Entity("Domain.Model.Schedule", b =>
                {
                    b.HasOne("Domain.Model.Account", "AssignedToNavigation")
                        .WithMany("Schedules")
                        .HasForeignKey("AssignedTo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Model.FarmActivity", "FarmActivity")
                        .WithMany("Schedules")
                        .HasForeignKey("FarmActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Model.FarmDetail", "FarmDetails")
                        .WithMany("Schedules")
                        .HasForeignKey("FarmDetailsId")
                        .IsRequired();

                    b.Navigation("AssignedToNavigation");

                    b.Navigation("FarmActivity");

                    b.Navigation("FarmDetails");
                });

            modelBuilder.Entity("Domain.Model.Account", b =>
                {
                    b.Navigation("AccountProfile");

                    b.Navigation("FarmDetails");

                    b.Navigation("Feedbacks");

                    b.Navigation("Orders");

                    b.Navigation("Reports");

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("Domain.Model.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Domain.Model.Crop", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Domain.Model.FarmActivity", b =>
                {
                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("Domain.Model.FarmDetail", b =>
                {
                    b.Navigation("Crops");

                    b.Navigation("IoTdevices");

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("Domain.Model.Order", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Domain.Model.Product", b =>
                {
                    b.Navigation("OrderDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
