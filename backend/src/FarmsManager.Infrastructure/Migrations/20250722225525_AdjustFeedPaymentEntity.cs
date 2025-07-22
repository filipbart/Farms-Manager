using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustFeedPaymentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "payment_id",
                schema: "farms_manager",
                table: "feed_invoice",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_feed_invoice_payment_id",
                schema: "farms_manager",
                table: "feed_invoice",
                column: "payment_id");

            migrationBuilder.AddForeignKey(
                name: "fk_feed_invoice_feed_payment_entity_payment_id",
                schema: "farms_manager",
                table: "feed_invoice",
                column: "payment_id",
                principalSchema: "farms_manager",
                principalTable: "feed_payment",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_feed_invoice_feed_payment_entity_payment_id",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropIndex(
                name: "ix_feed_invoice_payment_id",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropColumn(
                name: "payment_id",
                schema: "farms_manager",
                table: "feed_invoice");
        }
    }
}
