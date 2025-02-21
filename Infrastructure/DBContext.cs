using Azure.Core;
using Domain;
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
        public DbSet<Account> Users { get; set; }
        public DbSet<AccountProfile> WorkSheet { get; set; }
        public DbSet<Category> WorkSheetWork { get; set; }
        public DbSet<FarmActivity> Request { get; set; }
        public DbSet<FarmDetail> Punishments { get; set; }
        public DbSet<Feedback> Invoice { get; set; }
        public DbSet<IoTdevice> InvoiceDetails { get; set; }
        public DbSet<Order> Product { get; set; }
        public DbSet<OrderDetail> Imports { get; set; }
        public DbSet<Payment> Inventory { get; set; }
        public DbSet<Product> Voucher { get; set; }
        public DbSet<Report> ImportDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
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
