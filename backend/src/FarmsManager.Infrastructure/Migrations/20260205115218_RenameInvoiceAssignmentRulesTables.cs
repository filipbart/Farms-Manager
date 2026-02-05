using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameInvoiceAssignmentRulesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rules_tax_business_entity_tax_business",
                schema: "farms_manager",
                table: "InvoiceFarmAssignmentRules");

            migrationBuilder.DropTable(
                name: "invoice_assignment_rules",
                schema: "farms_manager");

            migrationBuilder.RenameTable(
                name: "InvoiceFarmAssignmentRules",
                schema: "farms_manager",
                newName: "invoice_farm_assignment_rules",
                newSchema: "farms_manager");

            migrationBuilder.CreateTable(
                name: "invoice_user_assignment_rules",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    assigned_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    include_keywords = table.Column<string>(type: "jsonb", nullable: false),
                    exclude_keywords = table.Column<string>(type: "jsonb", nullable: false),
                    tax_business_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    farm_ids = table.Column<string>(type: "jsonb", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_user_assignment_rules", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_user_assignment_rules_tax_business_entity_tax_busin",
                        column: x => x.tax_business_entity_id,
                        principalSchema: "farms_manager",
                        principalTable: "tax_business_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_invoice_user_assignment_rules_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoice_user_assignment_rules_user_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoice_user_assignment_rules_user_entity_assigned_user_id",
                        column: x => x.assigned_user_id,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invoice_user_assignment_rules_user_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_invoice_user_assignment_rules_assigned_user_id",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "assigned_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_user_assignment_rules_created_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_user_assignment_rules_deleted_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_user_assignment_rules_is_active",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_user_assignment_rules_is_active_priority",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                columns: new[] { "is_active", "priority" });

            migrationBuilder.CreateIndex(
                name: "ix_invoice_user_assignment_rules_modified_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_user_assignment_rules_priority",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_user_assignment_rules_tax_business_entity_id",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "tax_business_entity_id");

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rules_tax_business_entity_tax_busin",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rules_tax_business_entity_tax_busin",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules");

            migrationBuilder.DropTable(
                name: "invoice_user_assignment_rules",
                schema: "farms_manager");

            migrationBuilder.RenameTable(
                name: "invoice_farm_assignment_rules",
                schema: "farms_manager",
                newName: "InvoiceFarmAssignmentRules",
                newSchema: "farms_manager");

            migrationBuilder.CreateTable(
                name: "invoice_assignment_rules",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tax_business_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    exclude_keywords = table.Column<string>(type: "jsonb", nullable: false),
                    farm_ids = table.Column<string>(type: "jsonb", nullable: false),
                    include_keywords = table.Column<string>(type: "jsonb", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_assignment_rules", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_assignment_rules_tax_business_entity_tax_business_e",
                        column: x => x.tax_business_entity_id,
                        principalSchema: "farms_manager",
                        principalTable: "tax_business_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_invoice_assignment_rules_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoice_assignment_rules_user_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoice_assignment_rules_user_entity_assigned_user_id",
                        column: x => x.assigned_user_id,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invoice_assignment_rules_user_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_invoice_assignment_rules_assigned_user_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "assigned_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_assignment_rules_created_by",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_assignment_rules_deleted_by",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_assignment_rules_is_active",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_assignment_rules_is_active_priority",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                columns: new[] { "is_active", "priority" });

            migrationBuilder.CreateIndex(
                name: "ix_invoice_assignment_rules_modified_by",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_assignment_rules_priority",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_assignment_rules_tax_business_entity_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "tax_business_entity_id");

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rules_tax_business_entity_tax_business",
                schema: "farms_manager",
                table: "InvoiceFarmAssignmentRules",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
