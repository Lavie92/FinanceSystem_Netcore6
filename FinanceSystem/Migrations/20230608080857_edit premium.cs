using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSystem.Migrations
{
    public partial class editpremium : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "PremiumSubscriptions");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "PremiumSubscriptions");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "PremiumSubscriptions",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "PremiumSubscriptions");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "PremiumSubscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "PremiumSubscriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
