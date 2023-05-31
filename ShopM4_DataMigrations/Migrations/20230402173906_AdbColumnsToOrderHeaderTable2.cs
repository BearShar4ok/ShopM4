using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopM4_DataMigrations
{
    public partial class AdbColumnsToOrderHeaderTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TarnsactionId",
                table: "OrderHeader",
                newName: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "OrderHeader",
                newName: "TarnsactionId");
        }
    }
}
