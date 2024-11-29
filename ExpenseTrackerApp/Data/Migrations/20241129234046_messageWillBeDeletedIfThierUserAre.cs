using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerApp.Data.Migrations
{
    public partial class messageWillBeDeletedIfThierUserAre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_AspNetUsers_ApplicationUserId",
                table: "messages");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_AspNetUsers_ApplicationUserId",
                table: "messages",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_AspNetUsers_ApplicationUserId",
                table: "messages");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_AspNetUsers_ApplicationUserId",
                table: "messages",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
