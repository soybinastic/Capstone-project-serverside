using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class NewColumnsInCustomerOrderDetailsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankStatementImageFile",
                table: "CustomerOrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BarangayClearanceImageFile",
                table: "CustomerOrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernmentIdImageFile",
                table: "CustomerOrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NBIImageFile",
                table: "CustomerOrderDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankStatementImageFile",
                table: "CustomerOrderDetails");

            migrationBuilder.DropColumn(
                name: "BarangayClearanceImageFile",
                table: "CustomerOrderDetails");

            migrationBuilder.DropColumn(
                name: "GovernmentIdImageFile",
                table: "CustomerOrderDetails");

            migrationBuilder.DropColumn(
                name: "NBIImageFile",
                table: "CustomerOrderDetails");
        }
    }
}
