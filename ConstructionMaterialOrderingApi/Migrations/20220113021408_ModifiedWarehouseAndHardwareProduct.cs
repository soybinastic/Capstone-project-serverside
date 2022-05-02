using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ConstructionMaterialOrderingApi.Migrations
{
    public partial class ModifiedWarehouseAndHardwareProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedAt",
                table: "HardwareProducts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "HardwareProducts");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "HardwareProducts");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAt",
                table: "WarehouseProducts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "WarehouseProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "WarehouseProducts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedAt",
                table: "WarehouseProducts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "WarehouseProducts");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "WarehouseProducts");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAt",
                table: "HardwareProducts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "HardwareProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "HardwareProducts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
