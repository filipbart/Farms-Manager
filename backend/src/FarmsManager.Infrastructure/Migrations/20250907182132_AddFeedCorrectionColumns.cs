using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedCorrectionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "payment_date_utc",
                schema: "farms_manager",
                table: "feed_invoice_correction",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "payment_id",
                schema: "farms_manager",
                table: "feed_invoice_correction",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payment_date_utc",
                schema: "farms_manager",
                table: "feed_invoice_correction");

            migrationBuilder.DropColumn(
                name: "payment_id",
                schema: "farms_manager",
                table: "feed_invoice_correction");
        }
    }
}
