using Microsoft.EntityFrameworkCore;
using Proj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj.DataAccess.Data.DataSeeder
{
    public static class ProductSeeder
    {
        public static void Seed(ModelBuilder builder)
        {
            builder.Entity<Product>().HasData(
                new Product
                { 
                    Id=1,
                    Title = "Book_1",
                    Author = "Author_1",
                    Discription = "The Best Book Ever 1",
                    ISBN = "F0T00000001",
                    ListPrice = 89,
                    Price = 80,
                    Price50 = 75,
                    Price100 = 80,
                    CategoryId = 16,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 2,
                    Title = "Book_2",
                    Author = "Author_2",
                    Discription = "The Best Book Ever 2",
                    ISBN = "F0T00000002",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 16,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "Book_3",
                    Author = "Author_3",
                    Discription = "The Best Book Ever 3",
                    ISBN = "F0T00000003",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 16,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 4,
                    Title = "Book_4",
                    Author = "Author_4",
                    Discription = "The Best Book Ever 4",
                    ISBN = "F0T00000003",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 16,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 5,
                    Title = "Book_5",
                    Author = "Author_5",
                    Discription = "The Best Book Ever 5",
                    ISBN = "F0T00000005",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 16,
                    ImageUrl = ""
                }
              );
        }
    }
}
