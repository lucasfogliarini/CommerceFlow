using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Shipment");

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "Shipment",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Number",
                table: "Order",
                column: "Number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_Number",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Shipment");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Shipment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
