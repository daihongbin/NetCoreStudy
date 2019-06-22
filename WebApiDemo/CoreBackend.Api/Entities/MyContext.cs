using Microsoft.EntityFrameworkCore;

namespace CoreBackend.Api.Entities
{
    public class MyContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Material> Materials { get; set; }

        //第一种连接数据库的方法
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("xxxx connection string");
            base.OnConfiguring(optionsBuilder);
        }
        */

        //第二种方法
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
            Database.Migrate();
        }

        //使用Fluet Api
        //如果实体类过多，写在一个方法里不太利于维护
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(x => x.Id);
            modelBuilder.Entity<Product>().Property(x => x.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Product>().Property(x => x.Price).HasColumnType("decimal(8,2)");
        }
        */

        //改进后的配置实体类的方法
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new MaterialConfiguration());
        }
    }

    
}
