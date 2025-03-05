using Microsoft.EntityFrameworkCore;

namespace WebBanHang.Models
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Laptop" },
                new Category { Id = 2, Name = "Smartphone" },
                new Category { Id = 3, Name = "Tablet" }
            );

            modelBuilder.Entity<Product>().HasData(
                new {Id = 1, Name = "Dell XPS 13", Price = 2000m, Description = "123123", ImageUrl="lekrnwlkf",CategoryId = 1},
                new {Id = 2, Name = "iPhone 12", Price = 1000m, Description = "123123", ImageUrl="lưekrnwlkf",CategoryId = 2},
                new {Id = 3, Name = "iPad Pro", Price = 800m, Description = "123123", ImageUrl="lekrnwlkf",CategoryId = 3}
            );
        }
 
            
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}