using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class AddBestSellingReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BestSellingReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    BestSellingReportType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateReported = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestSellingReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BestSellingDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BestSellingReportId = table.Column<int>(type: "int", nullable: false),
                    BestSellingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestSellingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BestSellingDetails_BestSellingReports_BestSellingReportId",
                        column: x => x.BestSellingReportId,
                        principalTable: "BestSellingReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BestSellingProductReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BestSellingDetailId = table.Column<int>(type: "int", nullable: false),
                    SaleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestSellingProductReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BestSellingProductReports_BestSellingDetails_BestSellingDetailId",
                        column: x => x.BestSellingDetailId,
                        principalTable: "BestSellingDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BestSellingProductReports_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BestSellingDetails_BestSellingReportId",
                table: "BestSellingDetails",
                column: "BestSellingReportId");

            migrationBuilder.CreateIndex(
                name: "IX_BestSellingProductReports_BestSellingDetailId",
                table: "BestSellingProductReports",
                column: "BestSellingDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BestSellingProductReports_SaleId",
                table: "BestSellingProductReports",
                column: "SaleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BestSellingProductReports");

            migrationBuilder.DropTable(
                name: "BestSellingDetails");

            migrationBuilder.DropTable(
                name: "BestSellingReports");
        }
    }
}
