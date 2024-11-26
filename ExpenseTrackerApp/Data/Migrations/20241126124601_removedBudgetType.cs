using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerApp.Data.Migrations
{
    public partial class removedBudgetType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_budgets_budgettypes_BudgetTypeId",
                table: "budgets");

            migrationBuilder.DropTable(
                name: "budgettypes");

            migrationBuilder.DropIndex(
                name: "IX_budgets_BudgetTypeId",
                table: "budgets");

            migrationBuilder.DropColumn(
                name: "BudgetTypeId",
                table: "budgets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetTypeId",
                table: "budgets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "budgettypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budgettypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_budgets_BudgetTypeId",
                table: "budgets",
                column: "BudgetTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_budgets_budgettypes_BudgetTypeId",
                table: "budgets",
                column: "BudgetTypeId",
                principalTable: "budgettypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
