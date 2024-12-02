using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerApp.Data.Migrations
{
    public partial class updatedLinkPropertiesToMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LinkRepositoryLink",
                table: "messages",
                newName: "ControllerLink");

            migrationBuilder.RenameColumn(
                name: "LinkActionLink",
                table: "messages",
                newName: "ActionLink");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ControllerLink",
                table: "messages",
                newName: "LinkRepositoryLink");

            migrationBuilder.RenameColumn(
                name: "ActionLink",
                table: "messages",
                newName: "LinkActionLink");
        }
    }
}
