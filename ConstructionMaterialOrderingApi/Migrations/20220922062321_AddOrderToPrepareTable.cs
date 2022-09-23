using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class AddOrderToPrepareTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderToPrepares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesClerkId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderToPrepares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderToPrepares_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderToPrepares_SalesClerks_SalesClerkId",
                        column: x => x.SalesClerkId,
                        principalTable: "SalesClerks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderToPrepares_OrderId",
                table: "OrderToPrepares",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderToPrepares_SalesClerkId",
                table: "OrderToPrepares",
                column: "SalesClerkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderToPrepares");
        }
    }
}
