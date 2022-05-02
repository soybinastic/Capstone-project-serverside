using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class AddWarehouseReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WarehouseReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateReported = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseReports_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseProductStatusReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseReportId = table.Column<int>(type: "int", nullable: false),
                    HardwareProductId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseProductStatusReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseProductStatusReports_HardwareProducts_HardwareProductId",
                        column: x => x.HardwareProductId,
                        principalTable: "HardwareProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseProductStatusReports_WarehouseReports_WarehouseReportId",
                        column: x => x.WarehouseReportId,
                        principalTable: "WarehouseReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseProductStatusReports_HardwareProductId",
                table: "WarehouseProductStatusReports",
                column: "HardwareProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseProductStatusReports_WarehouseReportId",
                table: "WarehouseProductStatusReports",
                column: "WarehouseReportId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseReports_WarehouseId",
                table: "WarehouseReports",
                column: "WarehouseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseProductStatusReports");

            migrationBuilder.DropTable(
                name: "WarehouseReports");
        }
    }
}
