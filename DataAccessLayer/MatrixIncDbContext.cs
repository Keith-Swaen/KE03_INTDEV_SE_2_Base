using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class MatrixIncDbContext : DbContext
    {
        public MatrixIncDbContext(DbContextOptions<MatrixIncDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }

        // ✅ Nieuw toegevoegd
        public DbSet<OrderProduct> OrderProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relatie tussen Customer en Orders
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .IsRequired();

            // Relatie tussen OrderProduct en Order
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            // Relatie tussen OrderProduct en Product
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.SetNull); // Zet bij verwijderen van Product de verwijzing naar null


            // Relatie tussen Part en Products (many-to-many)
            modelBuilder.Entity<Part>()
                .HasMany(p => p.Products)
                .WithMany(p => p.Parts);

            // Relatie tussen Category en Products
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}