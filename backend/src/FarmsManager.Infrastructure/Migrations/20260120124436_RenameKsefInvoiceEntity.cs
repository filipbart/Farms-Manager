using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameKsefInvoiceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "k_se_f_number",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ksef_number");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_invoice_k_se_f_number",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ix_ksef_invoice_ksef_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ksef_number",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "k_se_f_number");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_invoice_ksef_number",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ix_ksef_invoice_k_se_f_number");
        }
    }
}
