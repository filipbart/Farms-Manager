using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameInvoiceAssignmentRulesTablesAndTaxBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_farm_tax_business_entity_tax_business_entity_id",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropForeignKey(
                name: "fk_ksef_invoice_tax_business_entity_tax_business_entity_id",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_module_assignment_rule_tax_business_entity_tax_busi",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rules_farm_entity_target_farm_id",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rules_tax_business_entity_tax_busin",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rules_user_created_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rules_user_deleted_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rules_user_modified_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_user_assignment_rules_tax_business_entity_tax_busin",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_user_assignment_rules_user_created_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_user_assignment_rules_user_deleted_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_user_assignment_rules_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_user_assignment_rules_user_modified_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_tax_business_entity_user_created_by",
                schema: "farms_manager",
                table: "tax_business_entity");

            migrationBuilder.DropForeignKey(
                name: "fk_tax_business_entity_user_deleted_by",
                schema: "farms_manager",
                table: "tax_business_entity");

            migrationBuilder.DropForeignKey(
                name: "fk_tax_business_entity_user_modified_by",
                schema: "farms_manager",
                table: "tax_business_entity");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tax_business_entity",
                schema: "farms_manager",
                table: "tax_business_entity");

            migrationBuilder.DropPrimaryKey(
                name: "pk_invoice_user_assignment_rules",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules");

            migrationBuilder.DropPrimaryKey(
                name: "pk_invoice_farm_assignment_rules",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules");

            migrationBuilder.RenameTable(
                name: "tax_business_entity",
                schema: "farms_manager",
                newName: "tax_business",
                newSchema: "farms_manager");

            migrationBuilder.RenameTable(
                name: "invoice_user_assignment_rules",
                schema: "farms_manager",
                newName: "invoice_user_assignment_rule",
                newSchema: "farms_manager");

            migrationBuilder.RenameTable(
                name: "invoice_farm_assignment_rules",
                schema: "farms_manager",
                newName: "invoice_farm_assignment_rule",
                newSchema: "farms_manager");

            migrationBuilder.RenameIndex(
                name: "ix_tax_business_entity_nip_name",
                schema: "farms_manager",
                table: "tax_business",
                newName: "ix_tax_business_nip_name");

            migrationBuilder.RenameIndex(
                name: "ix_tax_business_entity_nip",
                schema: "farms_manager",
                table: "tax_business",
                newName: "ix_tax_business_nip");

            migrationBuilder.RenameIndex(
                name: "ix_tax_business_entity_modified_by",
                schema: "farms_manager",
                table: "tax_business",
                newName: "ix_tax_business_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_tax_business_entity_deleted_by",
                schema: "farms_manager",
                table: "tax_business",
                newName: "ix_tax_business_deleted_by");

            migrationBuilder.RenameIndex(
                name: "ix_tax_business_entity_created_by",
                schema: "farms_manager",
                table: "tax_business",
                newName: "ix_tax_business_created_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rules_tax_business_entity_id",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                newName: "ix_invoice_user_assignment_rule_tax_business_entity_id");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rules_priority",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                newName: "ix_invoice_user_assignment_rule_priority");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rules_modified_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                newName: "ix_invoice_user_assignment_rule_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rules_is_active_priority",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                newName: "ix_invoice_user_assignment_rule_is_active_priority");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rules_is_active",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                newName: "ix_invoice_user_assignment_rule_is_active");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rules_deleted_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                newName: "ix_invoice_user_assignment_rule_deleted_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rules_created_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                newName: "ix_invoice_user_assignment_rule_created_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rules_assigned_user_id",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                newName: "ix_invoice_user_assignment_rule_assigned_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rules_tax_business_entity_id",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                newName: "ix_invoice_farm_assignment_rule_tax_business_entity_id");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rules_target_farm_id",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                newName: "ix_invoice_farm_assignment_rule_target_farm_id");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rules_priority",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                newName: "ix_invoice_farm_assignment_rule_priority");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rules_modified_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                newName: "ix_invoice_farm_assignment_rule_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rules_is_active",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                newName: "ix_invoice_farm_assignment_rule_is_active");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rules_deleted_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                newName: "ix_invoice_farm_assignment_rule_deleted_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rules_created_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                newName: "ix_invoice_farm_assignment_rule_created_by");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tax_business",
                schema: "farms_manager",
                table: "tax_business",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_invoice_user_assignment_rule",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_invoice_farm_assignment_rule",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_farm_tax_business_tax_business_entity_id",
                schema: "farms_manager",
                table: "farm",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rule_farm_entity_target_farm_id",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                column: "target_farm_id",
                principalSchema: "farms_manager",
                principalTable: "farm",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rule_tax_business_entity_tax_busine",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rule_user_created_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rule_user_deleted_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rule_user_modified_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_user_assignment_rule_tax_business_entity_tax_busine",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_user_assignment_rule_user_created_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_user_assignment_rule_user_deleted_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_user_assignment_rule_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                column: "assigned_user_id",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_user_assignment_rule_user_modified_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tax_business_user_created_by",
                schema: "farms_manager",
                table: "tax_business",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tax_business_user_deleted_by",
                schema: "farms_manager",
                table: "tax_business",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tax_business_user_modified_by",
                schema: "farms_manager",
                table: "tax_business",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_farm_tax_business_tax_business_entity_id",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rule_farm_entity_target_farm_id",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rule_tax_business_entity_tax_busine",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rule_user_created_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rule_user_deleted_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_farm_assignment_rule_user_modified_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_user_assignment_rule_tax_business_entity_tax_busine",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_user_assignment_rule_user_created_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_user_assignment_rule_user_deleted_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_user_assignment_rule_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_invoice_user_assignment_rule_user_modified_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_tax_business_user_created_by",
                schema: "farms_manager",
                table: "tax_business");

            migrationBuilder.DropForeignKey(
                name: "fk_tax_business_user_deleted_by",
                schema: "farms_manager",
                table: "tax_business");

            migrationBuilder.DropForeignKey(
                name: "fk_tax_business_user_modified_by",
                schema: "farms_manager",
                table: "tax_business");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tax_business",
                schema: "farms_manager",
                table: "tax_business");

            migrationBuilder.DropPrimaryKey(
                name: "pk_invoice_user_assignment_rule",
                schema: "farms_manager",
                table: "invoice_user_assignment_rule");

            migrationBuilder.DropPrimaryKey(
                name: "pk_invoice_farm_assignment_rule",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rule");

            migrationBuilder.RenameTable(
                name: "tax_business",
                schema: "farms_manager",
                newName: "tax_business_entity",
                newSchema: "farms_manager");

            migrationBuilder.RenameTable(
                name: "invoice_user_assignment_rule",
                schema: "farms_manager",
                newName: "invoice_user_assignment_rules",
                newSchema: "farms_manager");

            migrationBuilder.RenameTable(
                name: "invoice_farm_assignment_rule",
                schema: "farms_manager",
                newName: "invoice_farm_assignment_rules",
                newSchema: "farms_manager");

            migrationBuilder.RenameIndex(
                name: "ix_tax_business_nip_name",
                schema: "farms_manager",
                table: "tax_business_entity",
                newName: "ix_tax_business_entity_nip_name");

            migrationBuilder.RenameIndex(
                name: "ix_tax_business_nip",
                schema: "farms_manager",
                table: "tax_business_entity",
                newName: "ix_tax_business_entity_nip");

            migrationBuilder.RenameIndex(
                name: "ix_tax_business_modified_by",
                schema: "farms_manager",
                table: "tax_business_entity",
                newName: "ix_tax_business_entity_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_tax_business_deleted_by",
                schema: "farms_manager",
                table: "tax_business_entity",
                newName: "ix_tax_business_entity_deleted_by");

            migrationBuilder.RenameIndex(
                name: "ix_tax_business_created_by",
                schema: "farms_manager",
                table: "tax_business_entity",
                newName: "ix_tax_business_entity_created_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rule_tax_business_entity_id",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                newName: "ix_invoice_user_assignment_rules_tax_business_entity_id");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rule_priority",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                newName: "ix_invoice_user_assignment_rules_priority");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rule_modified_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                newName: "ix_invoice_user_assignment_rules_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rule_is_active_priority",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                newName: "ix_invoice_user_assignment_rules_is_active_priority");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rule_is_active",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                newName: "ix_invoice_user_assignment_rules_is_active");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rule_deleted_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                newName: "ix_invoice_user_assignment_rules_deleted_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rule_created_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                newName: "ix_invoice_user_assignment_rules_created_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_user_assignment_rule_assigned_user_id",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                newName: "ix_invoice_user_assignment_rules_assigned_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rule_tax_business_entity_id",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                newName: "ix_invoice_farm_assignment_rules_tax_business_entity_id");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rule_target_farm_id",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                newName: "ix_invoice_farm_assignment_rules_target_farm_id");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rule_priority",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                newName: "ix_invoice_farm_assignment_rules_priority");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rule_modified_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                newName: "ix_invoice_farm_assignment_rules_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rule_is_active",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                newName: "ix_invoice_farm_assignment_rules_is_active");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rule_deleted_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                newName: "ix_invoice_farm_assignment_rules_deleted_by");

            migrationBuilder.RenameIndex(
                name: "ix_invoice_farm_assignment_rule_created_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                newName: "ix_invoice_farm_assignment_rules_created_by");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tax_business_entity",
                schema: "farms_manager",
                table: "tax_business_entity",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_invoice_user_assignment_rules",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_invoice_farm_assignment_rules",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_farm_tax_business_entity_tax_business_entity_id",
                schema: "farms_manager",
                table: "farm",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rules_farm_entity_target_farm_id",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                column: "target_farm_id",
                principalSchema: "farms_manager",
                principalTable: "farm",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rules_tax_business_entity_tax_busin",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rules_user_created_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rules_user_deleted_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_farm_assignment_rules_user_modified_by",
                schema: "farms_manager",
                table: "invoice_farm_assignment_rules",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_user_assignment_rules_tax_business_entity_tax_busin",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_user_assignment_rules_user_created_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_user_assignment_rules_user_deleted_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_user_assignment_rules_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "assigned_user_id",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_user_assignment_rules_user_modified_by",
                schema: "farms_manager",
                table: "invoice_user_assignment_rules",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tax_business_entity_user_created_by",
                schema: "farms_manager",
                table: "tax_business_entity",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tax_business_entity_user_deleted_by",
                schema: "farms_manager",
                table: "tax_business_entity",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tax_business_entity_user_modified_by",
                schema: "farms_manager",
                table: "tax_business_entity",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_invoice_tax_business_entity_tax_business_entity_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_module_assignment_rule_tax_business_entity_tax_busi",
                schema: "farms_manager",
                table: "invoice_module_assignment_rule",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
