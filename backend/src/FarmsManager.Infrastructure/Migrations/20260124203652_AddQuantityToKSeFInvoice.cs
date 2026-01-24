using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityToKSeFInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "quantity",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantity",
                schema: "farms_manager",
                table: "ksef_invoice");
        }
    }
}
