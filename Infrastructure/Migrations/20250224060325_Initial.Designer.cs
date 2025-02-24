﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250224060325_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Account", b =>
                {
                    b.Property<long>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AccountId"));

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("Phone")
                        .HasColumnType("int");

                    b.Property<int?>("Role")
                        .HasColumnType("int");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateOnly?>("UpdatedAt")
                        .HasColumnType("date");

                    b.HasKey("AccountId");

                    b.ToTable("Account", (string)null);
                });

            modelBuilder.Entity("Domain.AccountProfile", b =>
                {
                    b.Property<long>("AccountProfileId")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("Images")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<DateOnly?>("UpdatedAt")
                        .HasColumnType("date");

                    b.HasKey("AccountProfileId");

                    b.ToTable("AccountProfile", (string)null);
                });

            modelBuilder.Entity("Domain.Category", b =>
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

            modelBuilder.Entity("Domain.FarmActivity", b =>
                {
                    b.Property<long>("FarmActivitiesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("FarmActivitiesId"));

                    b.Property<int?>("ActivityType")
                        .HasColumnType("int");

                    b.Property<long>("AssignedTo")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("date");

                    b.Property<long>("FarmId")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("StartDate")
                        .HasColumnType("date");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.HasKey("FarmActivitiesId");

                    b.HasIndex("AssignedTo");

                    b.HasIndex("FarmId");

                    b.ToTable("FarmActivity", (string)null);
                });

            modelBuilder.Entity("Domain.FarmDetail", b =>
                {
                    b.Property<long>("FarmDetailsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("FarmDetailsId"));

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

                    b.Property<long>("StaffId")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("UpdatedAt")
                        .HasColumnType("date");

                    b.HasKey("FarmDetailsId");

                    b.HasIndex("StaffId");

                    b.ToTable("FarmDetail", (string)null);
                });

            modelBuilder.Entity("Domain.Feedback", b =>
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

                    b.HasKey("FeedbackId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Feedback", (string)null);
                });

            modelBuilder.Entity("Domain.IoTdevice", b =>
                {
                    b.Property<long>("IoTdevicesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("IoTdevicesId"));

                    b.Property<int?>("DeviceName")
                        .HasColumnType("int");

                    b.Property<string>("DeviceType")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<long>("FarmId")
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

                    b.HasIndex("FarmId");

                    b.ToTable("IoTDevices", (string)null);
                });

            modelBuilder.Entity("Domain.Order", b =>
                {
                    b.Property<long>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("OrderId"));

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<long>("CustomerId")
                        .HasColumnType("bigint");

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

            modelBuilder.Entity("Domain.OrderDetail", b =>
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

            modelBuilder.Entity("Domain.Payment", b =>
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

            modelBuilder.Entity("Domain.Product", b =>
                {
                    b.Property<long>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ProductId"));

                    b.Property<long>("CategoryId")
                        .HasColumnType("bigint");

                    b.Property<DateOnly?>("CreatedAt")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StockQuantity")
                        .HasColumnType("int");

                    b.Property<DateOnly?>("UpdatedAt")
                        .HasColumnType("date");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Product", (string)null);
                });

            modelBuilder.Entity("Domain.Report", b =>
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

                    b.HasKey("ReportId");

                    b.HasIndex("GeneratedBy");

                    b.ToTable("Report", (string)null);
                });

            modelBuilder.Entity("Domain.AccountProfile", b =>
                {
                    b.HasOne("Domain.Account", "AccountProfileNavigation")
                        .WithOne("AccountProfile")
                        .HasForeignKey("Domain.AccountProfile", "AccountProfileId")
                        .IsRequired();

                    b.Navigation("AccountProfileNavigation");
                });

            modelBuilder.Entity("Domain.FarmActivity", b =>
                {
                    b.HasOne("Domain.Account", "AssignedToNavigation")
                        .WithMany("FarmActivities")
                        .HasForeignKey("AssignedTo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.FarmDetail", "Farm")
                        .WithMany("FarmActivities")
                        .HasForeignKey("FarmId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssignedToNavigation");

                    b.Navigation("Farm");
                });

            modelBuilder.Entity("Domain.FarmDetail", b =>
                {
                    b.HasOne("Domain.Account", "Staff")
                        .WithMany("FarmDetails")
                        .HasForeignKey("StaffId")
                        .IsRequired();

                    b.Navigation("Staff");
                });

            modelBuilder.Entity("Domain.Feedback", b =>
                {
                    b.HasOne("Domain.Account", "Customer")
                        .WithMany("Feedbacks")
                        .HasForeignKey("CustomerId")
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Domain.IoTdevice", b =>
                {
                    b.HasOne("Domain.FarmDetail", "Farm")
                        .WithMany("IoTdevices")
                        .HasForeignKey("FarmId")
                        .IsRequired();

                    b.Navigation("Farm");
                });

            modelBuilder.Entity("Domain.Order", b =>
                {
                    b.HasOne("Domain.Account", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Domain.OrderDetail", b =>
                {
                    b.HasOne("Domain.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .IsRequired();

                    b.HasOne("Domain.Product", "Product")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ProductId")
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Domain.Payment", b =>
                {
                    b.HasOne("Domain.Order", "Order")
                        .WithMany("Payments")
                        .HasForeignKey("OrderId")
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Domain.Product", b =>
                {
                    b.HasOne("Domain.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Domain.Report", b =>
                {
                    b.HasOne("Domain.Account", "GeneratedByNavigation")
                        .WithMany("Reports")
                        .HasForeignKey("GeneratedBy")
                        .IsRequired();

                    b.Navigation("GeneratedByNavigation");
                });

            modelBuilder.Entity("Domain.Account", b =>
                {
                    b.Navigation("AccountProfile");

                    b.Navigation("FarmActivities");

                    b.Navigation("FarmDetails");

                    b.Navigation("Feedbacks");

                    b.Navigation("Orders");

                    b.Navigation("Reports");
                });

            modelBuilder.Entity("Domain.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Domain.FarmDetail", b =>
                {
                    b.Navigation("FarmActivities");

                    b.Navigation("IoTdevices");
                });

            modelBuilder.Entity("Domain.Order", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Domain.Product", b =>
                {
                    b.Navigation("OrderDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
