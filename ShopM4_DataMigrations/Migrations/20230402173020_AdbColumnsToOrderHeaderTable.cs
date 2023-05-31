using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopM4_DataMigrations
{
    public partial class AdbColumnsToOrderHeaderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateExecution",
                table: "OrderHeader",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TarnsactionId",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateExecution",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "TarnsactionId",
                table: "OrderHeader");
        }
    }
}
