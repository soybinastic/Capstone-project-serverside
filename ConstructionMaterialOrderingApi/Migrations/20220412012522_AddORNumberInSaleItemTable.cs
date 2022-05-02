using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class AddORNumberInSaleItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ORNumber",
                table: "SaleItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ORNumber",
                table: "SaleItems");
        }
    }
}
