using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerApp.Data.Migrations
{
    public partial class addedUserToTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_AspNetUsers_ApplicationUserId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesColors_CategoryColorId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesIcons_CategoryIconId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesTypes_CategoryTypeId",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "transactions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_ApplicationUserId",
                table: "transactions",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_AspNetUsers_ApplicationUserId",
                table: "categories",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categoriesColors_CategoryColorId",
                table: "categories",
                column: "CategoryColorId",
                principalTable: "categoriesColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categoriesIcons_CategoryIconId",
                table: "categories",
                column: "CategoryIconId",
                principalTable: "categoriesIcons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categoriesTypes_CategoryTypeId",
                table: "categories",
                column: "CategoryTypeId",
                principalTable: "categoriesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_AspNetUsers_ApplicationUserId",
                table: "transactions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_AspNetUsers_ApplicationUserId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesColors_CategoryColorId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesIcons_CategoryIconId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categoriesTypes_CategoryTypeId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_AspNetUsers_ApplicationUserId",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_ApplicationUserId",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_AspNetUsers_ApplicationUserId",
                table: "categories",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_categories_categoriesTypes_CategoryTypeId",
                table: "categories",
                column: "CategoryTypeId",
                principalTable: "categoriesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
