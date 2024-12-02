using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerApp.Data.Migrations
{
    public partial class addedLinkPropertiesToMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkActionLink",
                table: "messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkRepositoryLink",
                table: "messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkActionLink",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "LinkRepositoryLink",
                table: "messages");
        }
    }
}
