using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseAdvancePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_permission_expense_advance_registry_entity_",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.DropTable(
                name: "expense_advance_registry",
                schema: "farms_manager");

            migrationBuilder.RenameColumn(
                name: "expense_advance_registry_id",
                schema: "farms_manager",
                table: "expense_advance_permission",
                newName: "employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseAdvancePermissions_UserRegistryType",
                schema: "farms_manager",
                table: "expense_advance_permission",
                newName: "IX_ExpenseAdvancePermissions_UserEmployeeType");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseAdvancePermissions_RegistryId",
                schema: "farms_manager",
                table: "expense_advance_permission",
                newName: "IX_ExpenseAdvancePermissions_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_permission_employee_employee_id",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "employee_id",
                principalSchema: "farms_manager",
                principalTable: "employee",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_permission_employee_employee_id",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                schema: "farms_manager",
                table: "expense_advance_permission",
                newName: "expense_advance_registry_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseAdvancePermissions_UserEmployeeType",
                schema: "farms_manager",
                table: "expense_advance_permission",
                newName: "IX_ExpenseAdvancePermissions_UserRegistryType");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseAdvancePermissions_EmployeeId",
                schema: "farms_manager",
                table: "expense_advance_permission",
                newName: "IX_ExpenseAdvancePermissions_RegistryId");

            migrationBuilder.CreateTable(
                name: "expense_advance_registry",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expense_advance_registry", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_permission_expense_advance_registry_entity_",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "expense_advance_registry_id",
                principalSchema: "farms_manager",
                principalTable: "expense_advance_registry",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
