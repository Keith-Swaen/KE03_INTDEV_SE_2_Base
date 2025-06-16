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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relatie tussen Customer en Orders configureren
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId).IsRequired();

            // Relatie tussen Product en Orders configureren (many-to-many)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Orders)
                .WithMany(o => o.Products);

            // Relatie tussen Part en Products configureren (many-to-many)
            modelBuilder.Entity<Part>()
                .HasMany(p => p.Products)
                .WithMany(p => p.Parts);

            // Relatie tussen Category en Products configureren
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
