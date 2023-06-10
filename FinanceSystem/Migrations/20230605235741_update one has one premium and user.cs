using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSystem.Migrations
{
    public partial class updateonehasonepremiumanduser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Premiums_AspNetUsers_UserId",
                table: "Premiums");

            migrationBuilder.DropIndex(
                name: "IX_Premiums_UserId",
                table: "Premiums");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Premiums");

            migrationBuilder.AddForeignKey(
                name: "FK_Premiums_AspNetUsers_Id",
                table: "Premiums",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Premiums_AspNetUsers_Id",
                table: "Premiums");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Premiums",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Premiums_UserId",
                table: "Premiums",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Premiums_AspNetUsers_UserId",
                table: "Premiums",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
