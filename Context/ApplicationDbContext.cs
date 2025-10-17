using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockManagement.Entities;

namespace StockManagement.Context
{
    public class ApplicationDbContext : IdentityDbContext<Customer>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Addresses)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<Stock>()
                .HasMany(s => s.OrderDetails)
                .WithOne(od => od.Stock)
                .HasForeignKey(od => od.StockId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryAddress)
                .WithMany()
                .HasForeignKey(o => o.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.InvoiceAddress)
                .WithMany()
                .HasForeignKey(o => o.InvoiceAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}