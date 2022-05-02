using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class AddImageFileColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFile",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFile",
                table: "HardwareProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BestSellingReports_BranchId",
                table: "BestSellingReports",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BestSellingReports_WarehouseId",
                table: "BestSellingReports",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_BestSellingReports_Branches_BranchId",
                table: "BestSellingReports",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BestSellingReports_Warehouses_WarehouseId",
                table: "BestSellingReports",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BestSellingReports_Branches_BranchId",
                table: "BestSellingReports");

            migrationBuilder.DropForeignKey(
                name: "FK_BestSellingReports_Warehouses_WarehouseId",
                table: "BestSellingReports");

            migrationBuilder.DropIndex(
                name: "IX_BestSellingReports_BranchId",
                table: "BestSellingReports");

            migrationBuilder.DropIndex(
                name: "IX_BestSellingReports_WarehouseId",
                table: "BestSellingReports");

            migrationBuilder.DropColumn(
                name: "ImageFile",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageFile",
                table: "HardwareProducts");
        }
    }
}
