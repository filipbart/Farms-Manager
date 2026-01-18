using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceAuditLogsAndAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "payment_date",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ksef_invoice_attachment",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    file_path = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    content_type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    uploaded_by = table.Column<Guid>(type: "uuid", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ksef_invoice_attachment", x => x.id);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_attachment_k_se_f_invoice_entity_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "farms_manager",
                        principalTable: "ksef_invoice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_attachment_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_attachment_user_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_attachment_user_entity_uploaded_by",
                        column: x => x.uploaded_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_attachment_user_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ksef_invoice_audit_log",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    previous_status = table.Column<int>(type: "integer", nullable: true),
                    new_status = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ksef_invoice_audit_log", x => x.id);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_audit_log_k_se_f_invoice_entity_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "farms_manager",
                        principalTable: "ksef_invoice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_audit_log_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_audit_log_user_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_audit_log_user_entity_user_id",
                        column: x => x.user_id,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_audit_log_user_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_payment_date",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "payment_date");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_attachment_created_by",
                schema: "farms_manager",
                table: "ksef_invoice_attachment",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_attachment_date_created_utc",
                schema: "farms_manager",
                table: "ksef_invoice_attachment",
                column: "date_created_utc");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_attachment_deleted_by",
                schema: "farms_manager",
                table: "ksef_invoice_attachment",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_attachment_invoice_id",
                schema: "farms_manager",
                table: "ksef_invoice_attachment",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_attachment_modified_by",
                schema: "farms_manager",
                table: "ksef_invoice_attachment",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_attachment_uploaded_by",
                schema: "farms_manager",
                table: "ksef_invoice_attachment",
                column: "uploaded_by");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_audit_log_action",
                schema: "farms_manager",
                table: "ksef_invoice_audit_log",
                column: "action");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_audit_log_created_by",
                schema: "farms_manager",
                table: "ksef_invoice_audit_log",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_audit_log_date_created_utc",
                schema: "farms_manager",
                table: "ksef_invoice_audit_log",
                column: "date_created_utc");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_audit_log_deleted_by",
                schema: "farms_manager",
                table: "ksef_invoice_audit_log",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_audit_log_invoice_id",
                schema: "farms_manager",
                table: "ksef_invoice_audit_log",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_audit_log_modified_by",
                schema: "farms_manager",
                table: "ksef_invoice_audit_log",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_audit_log_user_id",
                schema: "farms_manager",
                table: "ksef_invoice_audit_log",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ksef_invoice_attachment",
                schema: "farms_manager");

            migrationBuilder.DropTable(
                name: "ksef_invoice_audit_log",
                schema: "farms_manager");

            migrationBuilder.DropIndex(
                name: "ix_ksef_invoice_payment_date",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "payment_date",
                schema: "farms_manager",
                table: "ksef_invoice");
        }
    }
}
