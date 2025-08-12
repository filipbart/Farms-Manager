using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCycleIdColumnForFeedPaymentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "cycle_id",
                schema: "farms_manager",
                table: "feed_payment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_feed_payment_cycle_id",
                schema: "farms_manager",
                table: "feed_payment",
                column: "cycle_id");

            migrationBuilder.AddForeignKey(
                name: "fk_feed_payment_cycle_cycle_id",
                schema: "farms_manager",
                table: "feed_payment",
                column: "cycle_id",
                principalSchema: "farms_manager",
                principalTable: "cycle",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_feed_payment_cycle_cycle_id",
                schema: "farms_manager",
                table: "feed_payment");

            migrationBuilder.DropIndex(
                name: "ix_feed_payment_cycle_id",
                schema: "farms_manager",
                table: "feed_payment");

            migrationBuilder.DropColumn(
                name: "cycle_id",
                schema: "farms_manager",
                table: "feed_payment");
        }
    }
}
