using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class ModifiedPropertiesInCustomerOrderProductAndCustomerOrderProductHistoryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "CustomerOrderProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductBrand",
                table: "CustomerOrderProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductQuality",
                table: "CustomerOrderProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "CustomerOrderProductHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductBrand",
                table: "CustomerOrderProductHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductQuality",
                table: "CustomerOrderProductHistories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CustomerOrderProducts");

            migrationBuilder.DropColumn(
                name: "ProductBrand",
                table: "CustomerOrderProducts");

            migrationBuilder.DropColumn(
                name: "ProductQuality",
                table: "CustomerOrderProducts");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CustomerOrderProductHistories");

            migrationBuilder.DropColumn(
                name: "ProductBrand",
                table: "CustomerOrderProductHistories");

            migrationBuilder.DropColumn(
                name: "ProductQuality",
                table: "CustomerOrderProductHistories");
        }
    }
}
