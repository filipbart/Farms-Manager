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
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Drop indexes if they exist
                    DROP INDEX IF EXISTS farms_manager.""IX_ExpenseAdvancePermissions_UserEmployeeType"";
                    DROP INDEX IF EXISTS farms_manager.""IX_ExpenseAdvancePermissions_UserId"";
                    
                    -- Rename index if it exists
                    ALTER INDEX IF EXISTS farms_manager.""IX_ExpenseAdvancePermissions_EmployeeId"" 
                        RENAME TO ix_expense_advance_permission_employee_id;
                    
                    -- Create new index with filter
                    CREATE UNIQUE INDEX ""IX_ExpenseAdvancePermissions_UserEmployeeType"" 
                        ON farms_manager.expense_advance_permission (user_id, employee_id, permission_type)
                        WHERE date_deleted_utc IS NULL;
                END $$;
            ");
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
