using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSystem.Data.Migrations
{
    public partial class editUserIdtonvarchar450 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_AspNetUsers_UserId1",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId1",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Wallets");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Wallets",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_AspNetUsers_UserId",
                table: "Wallets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_AspNetUsers_UserId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Wallets",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Wallets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId1",
                table: "Wallets",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_AspNetUsers_UserId1",
                table: "Wallets",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
