using E_CommerceAPP.Models.Entities;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_CommerceAPP.Data
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
        {

        }

        public DbSet<Products> Products { get; set; }
        public DbSet<Categories> Categories { get; set; }
        
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Categories entity
            modelBuilder.Entity<Categories>()
                .HasKey(c => c.Category_ID);    
             modelBuilder.Entity<Products>()
        .HasKey(p => p.Product_ID); // Set Product_ID as primary key

            // Configure Products entity
            modelBuilder.Entity<Products>()
                .HasKey(p => p.Product_ID);

            modelBuilder.Entity<Categories>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Categories)
                .HasForeignKey(p => p.Category_ID)
                .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior if necessary;

            modelBuilder.Entity<Products>()
                .HasOne(p => p.Categories) // Product belongs to one Category
                .WithMany(c => c.Products) // Category has many Products
                .HasForeignKey(p => p.Category_ID); // Foreign key constraint
            modelBuilder.Entity<Categories>()
              .HasMany(c => c.Products)
              .WithOne(p => p.Categories)
              .HasForeignKey(p => p.Category_ID)
              .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior if necessary

            modelBuilder.Entity<Products>()
                .HasOne(p => p.Categories)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.Category_ID)
                .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior if necessary

            


        }
    }
}
