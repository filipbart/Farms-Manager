using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedInvoiceCorrectionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "invoice_correction_id",
                schema: "farms_manager",
                table: "feed_invoice",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "feed_invoice_correction",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "text", nullable: true),
                    sub_total = table.Column<decimal>(type: "numeric", nullable: false),
                    vat_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    invoice_total = table.Column<decimal>(type: "numeric", nullable: false),
                    file_path = table.Column<string>(type: "text", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feed_invoice_correction", x => x.id);
                    table.ForeignKey(
                        name: "fk_feed_invoice_correction_farm_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_feed_invoice_invoice_correction_id",
                schema: "farms_manager",
                table: "feed_invoice",
                column: "invoice_correction_id");

            migrationBuilder.CreateIndex(
                name: "ix_feed_invoice_correction_farm_id",
                schema: "farms_manager",
                table: "feed_invoice_correction",
                column: "farm_id");

            migrationBuilder.AddForeignKey(
                name: "fk_feed_invoice_feed_invoice_correction_invoice_correction_id",
                schema: "farms_manager",
                table: "feed_invoice",
                column: "invoice_correction_id",
                principalSchema: "farms_manager",
                principalTable: "feed_invoice_correction",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_feed_invoice_feed_invoice_correction_invoice_correction_id",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropTable(
                name: "feed_invoice_correction",
                schema: "farms_manager");

            migrationBuilder.DropIndex(
                name: "ix_feed_invoice_invoice_correction_id",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_correction_id",
                schema: "farms_manager",
                table: "feed_invoice");
        }
    }
}
