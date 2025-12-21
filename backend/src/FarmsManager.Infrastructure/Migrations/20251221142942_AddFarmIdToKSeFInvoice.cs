using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFarmIdToKSeFInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "farm_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_farm_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "farm_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_invoice_farm_entity_farm_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "farm_id",
                principalSchema: "farms_manager",
                principalTable: "farm",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ksef_invoice_farm_entity_farm_id",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropIndex(
                name: "ix_ksef_invoice_farm_id",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "farm_id",
                schema: "farms_manager",
                table: "ksef_invoice");
        }
    }
}
