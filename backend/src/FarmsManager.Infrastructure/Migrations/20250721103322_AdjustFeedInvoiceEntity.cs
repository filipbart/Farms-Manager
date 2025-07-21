using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustFeedInvoiceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "correct_unit_price",
                schema: "farms_manager",
                table: "feed_invoice",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "payment_date_utc",
                schema: "farms_manager",
                table: "feed_invoice",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "correct_unit_price",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropColumn(
                name: "payment_date_utc",
                schema: "farms_manager",
                table: "feed_invoice");
        }
    }
}
