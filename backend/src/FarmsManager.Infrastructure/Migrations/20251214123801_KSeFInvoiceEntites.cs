using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class KSeFInvoiceEntites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "k_se_f_invoice",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    k_se_f_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    invoice_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    seller_nip = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    seller_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    buyer_nip = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    buyer_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    invoice_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    payment_status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    payment_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    vat_deduction_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    module_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    gross_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    net_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    vat_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    invoice_direction = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    invoice_source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    related_invoice_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    related_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    invoice_xml = table.Column<string>(type: "text", nullable: false),
                    assigned_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_cycle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_entity_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_k_se_f_invoice", x => x.id);
                    table.ForeignKey(
                        name: "fk_k_se_f_invoice_cycle_entity_assigned_cycle_id",
                        column: x => x.assigned_cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_k_se_f_invoice_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_k_se_f_invoice_user_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_k_se_f_invoice_user_entity_assigned_user_id",
                        column: x => x.assigned_user_id,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_k_se_f_invoice_user_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_invoice_assigned_cycle_id",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "assigned_cycle_id");

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_invoice_assigned_user_id",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "assigned_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_invoice_created_by",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_invoice_deleted_by",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_invoice_invoice_date",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "invoice_date");

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_invoice_invoice_number",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "invoice_number");

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_invoice_k_se_f_number",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "k_se_f_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_invoice_modified_by",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "modified_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "k_se_f_invoice",
                schema: "farms_manager");
        }
    }
}
