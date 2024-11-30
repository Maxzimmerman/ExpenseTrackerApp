using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerApp.Data.Migrations
{
    public partial class addedMessageType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MessageTypeId",
                table: "messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "messageTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IconBackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messageTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_messages_MessageTypeId",
                table: "messages",
                column: "MessageTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_messageTypes_MessageTypeId",
                table: "messages",
                column: "MessageTypeId",
                principalTable: "messageTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_messageTypes_MessageTypeId",
                table: "messages");

            migrationBuilder.DropTable(
                name: "messageTypes");

            migrationBuilder.DropIndex(
                name: "IX_messages_MessageTypeId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "MessageTypeId",
                table: "messages");
        }
    }
}
