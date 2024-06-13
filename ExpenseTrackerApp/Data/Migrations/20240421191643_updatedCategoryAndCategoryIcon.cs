using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerApp.Data.Migrations
{
    public partial class updatedCategoryAndCategoryIcon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "categoriesIcons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "categories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_categories_ApplicationUserId",
                table: "categories",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_AspNetUsers_ApplicationUserId",
                table: "categories",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_AspNetUsers_ApplicationUserId",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_ApplicationUserId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "categoriesIcons");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "categories");
        }
    }
}
