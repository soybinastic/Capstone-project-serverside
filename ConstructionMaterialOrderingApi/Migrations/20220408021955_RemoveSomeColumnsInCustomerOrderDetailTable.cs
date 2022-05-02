using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class RemoveSomeColumnsInCustomerOrderDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "CustomerOrderDetails");

            migrationBuilder.DropColumn(
                name: "BankStatementImageFile",
                table: "CustomerOrderDetails");

            migrationBuilder.DropColumn(
                name: "BarangayClearanceImageFile",
                table: "CustomerOrderDetails");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "CustomerOrderDetails");

            migrationBuilder.DropColumn(
                name: "GovernmentIdImageFile",
                table: "CustomerOrderDetails");

            migrationBuilder.DropColumn(
                name: "NBIImageFile",
                table: "CustomerOrderDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "CustomerOrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "CustomerOrderDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
    }
}
