using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTablesNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_k_se_f_invoice_cycle_entity_assigned_cycle_id",
                schema: "farms_manager",
                table: "k_se_f_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_k_se_f_invoice_user_created_by",
                schema: "farms_manager",
                table: "k_se_f_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_k_se_f_invoice_user_deleted_by",
                schema: "farms_manager",
                table: "k_se_f_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_k_se_f_invoice_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "k_se_f_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_k_se_f_invoice_user_modified_by",
                schema: "farms_manager",
                table: "k_se_f_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_k_se_f_synchronization_log_user_entity_created_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log");

            migrationBuilder.DropForeignKey(
                name: "fk_k_se_f_synchronization_log_user_entity_deleted_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log");

            migrationBuilder.DropForeignKey(
                name: "fk_k_se_f_synchronization_log_user_entity_modified_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log");

            migrationBuilder.DropPrimaryKey(
                name: "pk_k_se_f_synchronization_log",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log");

            migrationBuilder.DropPrimaryKey(
                name: "pk_k_se_f_invoice",
                schema: "farms_manager",
                table: "k_se_f_invoice");

            migrationBuilder.RenameTable(
                name: "k_se_f_synchronization_log",
                schema: "farms_manager",
                newName: "ksef_synchronization_log",
                newSchema: "farms_manager");

            migrationBuilder.RenameTable(
                name: "k_se_f_invoice",
                schema: "farms_manager",
                newName: "ksef_invoice",
                newSchema: "farms_manager");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_synchronization_log_status",
                schema: "farms_manager",
                table: "ksef_synchronization_log",
                newName: "ix_ksef_synchronization_log_status");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_synchronization_log_started_at",
                schema: "farms_manager",
                table: "ksef_synchronization_log",
                newName: "ix_ksef_synchronization_log_started_at");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_synchronization_log_modified_by",
                schema: "farms_manager",
                table: "ksef_synchronization_log",
                newName: "ix_ksef_synchronization_log_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_synchronization_log_deleted_by",
                schema: "farms_manager",
                table: "ksef_synchronization_log",
                newName: "ix_ksef_synchronization_log_deleted_by");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_synchronization_log_created_by",
                schema: "farms_manager",
                table: "ksef_synchronization_log",
                newName: "ix_ksef_synchronization_log_created_by");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_invoice_modified_by",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ix_ksef_invoice_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_invoice_k_se_f_number",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ix_ksef_invoice_k_se_f_number");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_invoice_invoice_number",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ix_ksef_invoice_invoice_number");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_invoice_invoice_date",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ix_ksef_invoice_invoice_date");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_invoice_deleted_by",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ix_ksef_invoice_deleted_by");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_invoice_created_by",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ix_ksef_invoice_created_by");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_invoice_assigned_user_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ix_ksef_invoice_assigned_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_k_se_f_invoice_assigned_cycle_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                newName: "ix_ksef_invoice_assigned_cycle_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ksef_synchronization_log",
                schema: "farms_manager",
                table: "ksef_synchronization_log",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ksef_invoice",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_invoice_cycle_entity_assigned_cycle_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "assigned_cycle_id",
                principalSchema: "farms_manager",
                principalTable: "cycle",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_invoice_user_created_by",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_invoice_user_deleted_by",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_invoice_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "assigned_user_id",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_invoice_user_modified_by",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_synchronization_log_user_entity_created_by",
                schema: "farms_manager",
                table: "ksef_synchronization_log",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_synchronization_log_user_entity_deleted_by",
                schema: "farms_manager",
                table: "ksef_synchronization_log",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_synchronization_log_user_entity_modified_by",
                schema: "farms_manager",
                table: "ksef_synchronization_log",
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
                name: "fk_ksef_invoice_cycle_entity_assigned_cycle_id",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_ksef_invoice_user_created_by",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_ksef_invoice_user_deleted_by",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_ksef_invoice_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_ksef_invoice_user_modified_by",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_ksef_synchronization_log_user_entity_created_by",
                schema: "farms_manager",
                table: "ksef_synchronization_log");

            migrationBuilder.DropForeignKey(
                name: "fk_ksef_synchronization_log_user_entity_deleted_by",
                schema: "farms_manager",
                table: "ksef_synchronization_log");

            migrationBuilder.DropForeignKey(
                name: "fk_ksef_synchronization_log_user_entity_modified_by",
                schema: "farms_manager",
                table: "ksef_synchronization_log");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ksef_synchronization_log",
                schema: "farms_manager",
                table: "ksef_synchronization_log");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ksef_invoice",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.RenameTable(
                name: "ksef_synchronization_log",
                schema: "farms_manager",
                newName: "k_se_f_synchronization_log",
                newSchema: "farms_manager");

            migrationBuilder.RenameTable(
                name: "ksef_invoice",
                schema: "farms_manager",
                newName: "k_se_f_invoice",
                newSchema: "farms_manager");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_synchronization_log_status",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                newName: "ix_k_se_f_synchronization_log_status");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_synchronization_log_started_at",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                newName: "ix_k_se_f_synchronization_log_started_at");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_synchronization_log_modified_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                newName: "ix_k_se_f_synchronization_log_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_synchronization_log_deleted_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                newName: "ix_k_se_f_synchronization_log_deleted_by");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_synchronization_log_created_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                newName: "ix_k_se_f_synchronization_log_created_by");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_invoice_modified_by",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                newName: "ix_k_se_f_invoice_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_invoice_k_se_f_number",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                newName: "ix_k_se_f_invoice_k_se_f_number");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_invoice_invoice_number",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                newName: "ix_k_se_f_invoice_invoice_number");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_invoice_invoice_date",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                newName: "ix_k_se_f_invoice_invoice_date");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_invoice_deleted_by",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                newName: "ix_k_se_f_invoice_deleted_by");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_invoice_created_by",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                newName: "ix_k_se_f_invoice_created_by");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_invoice_assigned_user_id",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                newName: "ix_k_se_f_invoice_assigned_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_ksef_invoice_assigned_cycle_id",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                newName: "ix_k_se_f_invoice_assigned_cycle_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_k_se_f_synchronization_log",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_k_se_f_invoice",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_k_se_f_invoice_cycle_entity_assigned_cycle_id",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "assigned_cycle_id",
                principalSchema: "farms_manager",
                principalTable: "cycle",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_k_se_f_invoice_user_created_by",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_k_se_f_invoice_user_deleted_by",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_k_se_f_invoice_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "assigned_user_id",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_k_se_f_invoice_user_modified_by",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_k_se_f_synchronization_log_user_entity_created_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_k_se_f_synchronization_log_user_entity_deleted_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_k_se_f_synchronization_log_user_entity_modified_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
