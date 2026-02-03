using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyFieldsToKSeFInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "currency_code",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "exchange_rate",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "original_gross_amount",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "original_net_amount",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "original_vat_amount",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "currency_code",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "exchange_rate",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "original_gross_amount",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "original_net_amount",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "original_vat_amount",
                schema: "farms_manager",
                table: "ksef_invoice");
        }
    }
}
