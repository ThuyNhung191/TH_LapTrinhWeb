using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanHang.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CachThanhToan",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CachVanChuyen",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DienThoai",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoTen",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaTrangThai",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayCan",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayGiao",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PhiVanChuyen",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CachThanhToan",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CachVanChuyen",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DienThoai",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HoTen",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MaTrangThai",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "NgayCan",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "NgayGiao",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PhiVanChuyen",
                table: "Orders");
        }
    }
}
