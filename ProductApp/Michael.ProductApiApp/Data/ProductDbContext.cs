using Michael.ProductApiApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Michael.ProductApiApp.Data
{
    public class ProductDbContext:DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            :base(options)
        {
            
        }
        
        public DbSet<Goods> Goods { get; set; }
        
        public DbSet<Category> Category { get; set; }
    }
}