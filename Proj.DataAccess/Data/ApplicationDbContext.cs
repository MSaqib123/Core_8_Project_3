using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proj.DataAccess.Data.DataSeeder;
using Proj.Models;

namespace Proj.DataAccess.Data
{
    public class ApplicationDbContext:IdentityDbContext<IdentityUser>//DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option):base(option)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> orderHeaders{ get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }


        //__________________ 1. Data Seeding ____________________________
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Category>().HasData(
        //        new Category { CategoryId = 1 , Name ="Mobile" , DisplayOrder = 1},
        //        new Category { CategoryId = 2 , Name ="Fruites" , DisplayOrder = 2},
        //        new Category { CategoryId = 3 , Name ="Laptops" , DisplayOrder = 2},
        //        new Category { CategoryId = 4 , Name ="Electronics" , DisplayOrder = 2},
        //        new Category { CategoryId = 5 , Name ="Dry Fruites" , DisplayOrder = 2},
        //        new Category { CategoryId = 6 , Name ="Pots" , DisplayOrder = 2},
        //        new Category { CategoryId = 7 , Name ="Vegitabes" , DisplayOrder = 2}
        //    );
        //}

        //__________________ 1. Data Seeding in Seprate Class ____________________________
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //________ Category Seeder ________________
            CategorySeeder.Seed(modelBuilder);

            //________ Product Seeder ________________
            ProductSeeder.Seed(modelBuilder);

            //________ Company Seeder ________________
            CompanySeeder.Seed(modelBuilder);

            //________ Identity DbContext Required This ________________
            base.OnModelCreating(modelBuilder);
        }
    }
}
