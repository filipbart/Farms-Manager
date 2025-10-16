using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleInvoiceStatusAndComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "comment",
                schema: "farms_manager",
                table: "sale_invoice",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "farms_manager",
                table: "sale_invoice",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Unrealized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comment",
                schema: "farms_manager",
                table: "sale_invoice");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "farms_manager",
                table: "sale_invoice");
        }
    }
}
