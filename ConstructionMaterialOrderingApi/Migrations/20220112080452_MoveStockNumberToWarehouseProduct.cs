using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class MoveStockNumberToWarehouseProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockNumber",
                table: "HardwareProducts");

            migrationBuilder.AddColumn<int>(
                name: "StockNumber",
                table: "WarehouseProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockNumber",
                table: "WarehouseProducts");

            migrationBuilder.AddColumn<int>(
                name: "StockNumber",
                table: "HardwareProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
