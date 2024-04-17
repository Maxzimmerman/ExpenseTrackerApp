using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerApp.Data.Migrations
{
    public partial class addedcategoriesandtransactionsupdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryColorId",
                table: "categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryIconId",
                table: "categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CateogryTypeId",
                table: "categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_categories_CategoryColorId",
                table: "categories",
                column: "CategoryColorId");

            migrationBuilder.CreateIndex(
                name: "IX_categories_CategoryIconId",
                table: "categories",
                column: "CategoryIconId");

            migrationBuilder.CreateIndex(
                name: "IX_categories_CateogryTypeId",
                table: "categories",
                column: "CateogryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categoriesColors_CategoryColorId",
                table: "categories",
                column: "CategoryColorId",
                principalTable: "categoriesColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categoriesIcons_CategoryIconId",
                table: "categories",
                column: "CategoryIconId",
                principalTable: "categoriesIcons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categoriesTypes_CateogryTypeId",
                table: "categories",
                column: "CateogryTypeId",
                principalTable: "categoriesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesColors_CategoryColorId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesIcons_CategoryIconId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesTypes_CateogryTypeId",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_CategoryColorId",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_CategoryIconId",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_CateogryTypeId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "CategoryColorId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "CategoryIconId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "CateogryTypeId",
                table: "categories");
        }
    }
}
