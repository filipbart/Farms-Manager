using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertFarmIdToFarmIdsInInvoiceAssignmentRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Usuń foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "fk_invoice_assignment_rules_farms_farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules");

            // Usuń index
            migrationBuilder.DropIndex(
                name: "ix_invoice_assignment_rules_farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules");

            // Dodaj nową kolumnę farm_ids jako jsonb
            migrationBuilder.AddColumn<string>(
                name: "farm_ids",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");

            // Migruj dane: jeśli farm_id ma wartość, przekonwertuj na tablicę z jednym elementem
            migrationBuilder.Sql(@"
                UPDATE farms_manager.invoice_assignment_rules
                SET farm_ids = jsonb_build_array(farm_id::text)
                WHERE farm_id IS NOT NULL;
            ");

            // Usuń starą kolumnę farm_id
            migrationBuilder.DropColumn(
                name: "farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Dodaj z powrotem kolumnę farm_id
            migrationBuilder.AddColumn<Guid>(
                name: "farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                type: "uuid",
                nullable: true);

            // Migruj dane z powrotem: weź pierwszy element z tablicy farm_ids
            migrationBuilder.Sql(@"
                UPDATE farms_manager.invoice_assignment_rules
                SET farm_id = (farm_ids->0)::text::uuid
                WHERE jsonb_array_length(farm_ids) > 0;
            ");

            // Usuń kolumnę farm_ids
            migrationBuilder.DropColumn(
                name: "farm_ids",
                schema: "farms_manager",
                table: "invoice_assignment_rules");

            // Przywróć index
            migrationBuilder.CreateIndex(
                name: "ix_invoice_assignment_rules_farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "farm_id");

            // Przywróć foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "fk_invoice_assignment_rules_farms_farm_id",
                schema: "farms_manager",
                table: "invoice_assignment_rules",
                column: "farm_id",
                principalSchema: "farms_manager",
                principalTable: "farms",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
