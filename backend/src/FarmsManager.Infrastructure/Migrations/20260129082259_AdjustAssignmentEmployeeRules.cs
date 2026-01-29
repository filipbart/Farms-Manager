using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustAssignmentEmployeeRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_invoice_assignment_rules_farm_entity_farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules");

            migrationBuilder.DropIndex(
                name: "ix_invoice_assignment_rules_farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules");

            migrationBuilder.DropColumn(
                name: "farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules");

            migrationBuilder.AddColumn<string>(
                name: "farm_ids",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "farm_ids",
                schema: "farms_manager",
                table: "invoice_assignment_rules");

            migrationBuilder.AddColumn<Guid>(
                name: "farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoice_assignment_rules_farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "farm_id");

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_assignment_rules_farm_entity_farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "farm_id",
                principalSchema: "farms_manager",
                principalTable: "farm",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
