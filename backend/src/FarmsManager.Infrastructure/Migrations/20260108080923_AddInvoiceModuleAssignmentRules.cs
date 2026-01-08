using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceModuleAssignmentRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invoice_module_assignment_rule",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    target_module = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    include_keywords = table.Column<string>(type: "jsonb", nullable: false),
                    exclude_keywords = table.Column<string>(type: "jsonb", nullable: false),
                    tax_business_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: true),
                    invoice_direction = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_invoice_module_assignment_rule", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_module_assignment_rule_farm_entity_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_invoice_module_assignment_rule_tax_business_entity_tax_busi",
                        column: x => x.tax_business_entity_id,
                        principalSchema: "farms_manager",
                        principalTable: "tax_business_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_invoice_module_assignment_rule_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoice_module_assignment_rule_user_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoice_module_assignment_rule_user_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_invoice_module_assignment_rule_created_by",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_module_assignment_rule_deleted_by",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_module_assignment_rule_farm_id",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_module_assignment_rule_is_active",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_module_assignment_rule_is_active_priority",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule",
                columns: new[] { "is_active", "priority" });

            migrationBuilder.CreateIndex(
                name: "ix_invoice_module_assignment_rule_modified_by",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_module_assignment_rule_priority",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_module_assignment_rule_target_module",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule",
                column: "target_module");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_module_assignment_rule_tax_business_entity_id",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule",
                column: "tax_business_entity_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_module_assignment_rule",
                schema: "farms_manager");
        }
    }
}
