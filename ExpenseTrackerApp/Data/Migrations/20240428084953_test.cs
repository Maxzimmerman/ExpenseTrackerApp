using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerApp.Data.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesTypes_CateogryTypeId",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "CateogryTypeId",
                table: "categories",
                newName: "CategoryTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_categories_CateogryTypeId",
                table: "categories",
                newName: "IX_categories_CategoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categoriesTypes_CategoryTypeId",
                table: "categories",
                column: "CategoryTypeId",
                principalTable: "categoriesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesTypes_CategoryTypeId",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "CategoryTypeId",
                table: "categories",
                newName: "CateogryTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_categories_CategoryTypeId",
                table: "categories",
                newName: "IX_categories_CateogryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categoriesTypes_CateogryTypeId",
                table: "categories",
                column: "CateogryTypeId",
                principalTable: "categoriesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
