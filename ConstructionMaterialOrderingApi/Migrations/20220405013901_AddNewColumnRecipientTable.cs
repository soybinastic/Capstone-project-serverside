using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class AddNewColumnRecipientTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Recipients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_OrderId",
                table: "Recipients",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipients_Orders_OrderId",
                table: "Recipients",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipients_Orders_OrderId",
                table: "Recipients");

            migrationBuilder.DropIndex(
                name: "IX_Recipients_OrderId",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Recipients");
        }
    }
}
