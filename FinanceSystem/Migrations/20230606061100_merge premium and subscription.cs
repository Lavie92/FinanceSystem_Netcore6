using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSystem.Migrations
{
    public partial class mergepremiumandsubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Premiums_AspNetUsers_Id",
                table: "Premiums");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Premiums",
                table: "Premiums");

            migrationBuilder.RenameTable(
                name: "Premiums",
                newName: "PremiumSubscriptions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PremiumSubscriptions",
                table: "PremiumSubscriptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PremiumSubscriptions_AspNetUsers_Id",
                table: "PremiumSubscriptions",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PremiumSubscriptions_AspNetUsers_Id",
                table: "PremiumSubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PremiumSubscriptions",
                table: "PremiumSubscriptions");

            migrationBuilder.RenameTable(
                name: "PremiumSubscriptions",
                newName: "Premiums");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Premiums",
                table: "Premiums",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Premiums_AspNetUsers_Id",
                table: "Premiums",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
