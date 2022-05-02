using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class AddHardwareStoreIdInWarehouseReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HardwareStoreId",
                table: "WarehouseReports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HardwareStoreId",
                table: "WarehouseReports");
        }
    }
}
