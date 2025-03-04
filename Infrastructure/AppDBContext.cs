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
