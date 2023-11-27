using Proj.Models;
using Microsoft.EntityFrameworkCore;

namespace Proj.DataAccess.Data.DataSeeder
{
    public static class CategorySeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Mobile", DisplayOrder = 1 },
                new Category { CategoryId = 2 , Name ="Fruites" , DisplayOrder = 2},
                new Category { CategoryId = 3, Name = "Laptops", DisplayOrder = 2 },
                new Category { CategoryId = 4, Name = "Electronics", DisplayOrder = 2 },
                new Category { CategoryId = 5, Name = "Dry Fruites", DisplayOrder = 2 },
                new Category { CategoryId = 6, Name = "Pots", DisplayOrder = 2 },
                new Category { CategoryId = 7, Name = "Vegitabes", DisplayOrder = 2 }
            );
        }
    }
}
