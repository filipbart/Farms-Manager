using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleInvoiceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sale_invoice",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    slaughterhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "text", nullable: true),
                    invoice_date = table.Column<DateOnly>(type: "date", nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    invoice_total = table.Column<decimal>(type: "numeric", nullable: false),
                    sub_total = table.Column<decimal>(type: "numeric", nullable: false),
                    vat_amount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("pk_sale_invoice", x => x.id);
                    table.ForeignKey(
                        name: "fk_sale_invoice_cycle_cycle_id",
                        column: x => x.cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sale_invoice_farm_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sale_invoice_slaughterhouse_entity_slaughterhouse_id",
                        column: x => x.slaughterhouse_id,
                        principalSchema: "farms_manager",
                        principalTable: "slaughterhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sale_invoice_cycle_id",
                schema: "farms_manager",
                table: "sale_invoice",
                column: "cycle_id");

            migrationBuilder.CreateIndex(
                name: "ix_sale_invoice_farm_id",
                schema: "farms_manager",
                table: "sale_invoice",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_sale_invoice_slaughterhouse_id",
                schema: "farms_manager",
                table: "sale_invoice",
                column: "slaughterhouse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sale_invoice",
                schema: "farms_manager");
        }
    }
}
