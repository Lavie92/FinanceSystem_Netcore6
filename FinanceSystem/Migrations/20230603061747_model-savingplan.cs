using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSystem.Migrations
{
    public partial class modelsavingplan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetSavingId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TargetSaving",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SaveDateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SaveDateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetSaving", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetSaving_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TargetSavingId",
                table: "Transactions",
                column: "TargetSavingId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetSaving_UserId",
                table: "TargetSaving",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TargetSaving_TargetSavingId",
                table: "Transactions",
                column: "TargetSavingId",
                principalTable: "TargetSaving",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TargetSaving_TargetSavingId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "TargetSaving");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TargetSavingId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TargetSavingId",
                table: "Transactions");
        }
    }
}
