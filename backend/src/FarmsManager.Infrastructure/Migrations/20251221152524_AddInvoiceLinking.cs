using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceLinking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "linking_accepted",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "linking_reminder_count",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "linking_reminder_date",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "requires_linking",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ksef_invoice_relation",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    relation_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ksef_invoice_relation", x => x.id);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_relation_ksef_invoice_source_invoice_id",
                        column: x => x.source_invoice_id,
                        principalSchema: "farms_manager",
                        principalTable: "ksef_invoice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_relation_ksef_invoice_target_invoice_id",
                        column: x => x.target_invoice_id,
                        principalSchema: "farms_manager",
                        principalTable: "ksef_invoice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_relation_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_relation_user_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ksef_invoice_relation_user_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_requires_linking",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "requires_linking");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_relation_created_by",
                schema: "farms_manager",
                table: "ksef_invoice_relation",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_relation_deleted_by",
                schema: "farms_manager",
                table: "ksef_invoice_relation",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_relation_modified_by",
                schema: "farms_manager",
                table: "ksef_invoice_relation",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_relation_source_invoice_id",
                schema: "farms_manager",
                table: "ksef_invoice_relation",
                column: "source_invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_relation_source_invoice_id_target_invoice_id",
                schema: "farms_manager",
                table: "ksef_invoice_relation",
                columns: new[] { "source_invoice_id", "target_invoice_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_relation_target_invoice_id",
                schema: "farms_manager",
                table: "ksef_invoice_relation",
                column: "target_invoice_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ksef_invoice_relation",
                schema: "farms_manager");

            migrationBuilder.DropIndex(
                name: "ix_ksef_invoice_requires_linking",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "linking_accepted",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "linking_reminder_count",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "linking_reminder_date",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "requires_linking",
                schema: "farms_manager",
                table: "ksef_invoice");
        }
    }
}
