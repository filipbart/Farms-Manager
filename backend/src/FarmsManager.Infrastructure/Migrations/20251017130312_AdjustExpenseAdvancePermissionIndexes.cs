using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustExpenseAdvancePermissionIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExpenseAdvancePermissions_UserEmployeeType",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseAdvancePermissions_UserId",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseAdvancePermissions_EmployeeId",
                schema: "farms_manager",
                table: "expense_advance_permission",
                newName: "ix_expense_advance_permission_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAdvancePermissions_UserEmployeeType",
                schema: "farms_manager",
                table: "expense_advance_permission",
                columns: new[] { "user_id", "employee_id", "permission_type" },
                unique: true,
                filter: "date_deleted_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExpenseAdvancePermissions_UserEmployeeType",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.RenameIndex(
                name: "ix_expense_advance_permission_employee_id",
                schema: "farms_manager",
                table: "expense_advance_permission",
                newName: "IX_ExpenseAdvancePermissions_EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAdvancePermissions_UserEmployeeType",
                schema: "farms_manager",
                table: "expense_advance_permission",
                columns: new[] { "user_id", "employee_id", "permission_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAdvancePermissions_UserId",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "user_id");
        }
    }
}
