using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class ModifiedRecieveProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HardwareStoreId",
                table: "RecieveProducts");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "RecieveProducts");

            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                table: "RecieveProducts",
                newName: "WarehouseReportId");

            migrationBuilder.CreateIndex(
                name: "IX_RecieveProducts_HardwareProductId",
                table: "RecieveProducts",
                column: "HardwareProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RecieveProducts_WarehouseReportId",
                table: "RecieveProducts",
                column: "WarehouseReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecieveProducts_HardwareProducts_HardwareProductId",
                table: "RecieveProducts",
                column: "HardwareProductId",
                principalTable: "HardwareProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecieveProducts_WarehouseReports_WarehouseReportId",
                table: "RecieveProducts",
                column: "WarehouseReportId",
                principalTable: "WarehouseReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecieveProducts_HardwareProducts_HardwareProductId",
                table: "RecieveProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_RecieveProducts_WarehouseReports_WarehouseReportId",
                table: "RecieveProducts");

            migrationBuilder.DropIndex(
                name: "IX_RecieveProducts_HardwareProductId",
                table: "RecieveProducts");

            migrationBuilder.DropIndex(
                name: "IX_RecieveProducts_WarehouseReportId",
                table: "RecieveProducts");

            migrationBuilder.RenameColumn(
                name: "WarehouseReportId",
                table: "RecieveProducts",
                newName: "WarehouseId");

            migrationBuilder.AddColumn<int>(
                name: "HardwareStoreId",
                table: "RecieveProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "RecieveProducts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
