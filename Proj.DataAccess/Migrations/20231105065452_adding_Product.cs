using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Proj.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class adding_Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Discription", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "Author_1", "The Best Book Ever 1", "F0T00000001", 89.0, 80.0, 80.0, 75.0, "Book_1" },
                    { 2, "Author_2", "The Best Book Ever 2", "F0T00000002", 99.0, 90.0, 80.0, 85.0, "Book_2" },
                    { 3, "Author_3", "The Best Book Ever 3", "F0T00000003", 99.0, 90.0, 80.0, 85.0, "Book_3" },
                    { 4, "Author_4", "The Best Book Ever 4", "F0T00000003", 99.0, 90.0, 80.0, 85.0, "Book_4" },
                    { 5, "Author_5", "The Best Book Ever 5", "F0T00000005", 99.0, 90.0, 80.0, 85.0, "Book_5" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
