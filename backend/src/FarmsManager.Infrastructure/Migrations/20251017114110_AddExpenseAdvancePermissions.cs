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
            // Check if table exists and handle accordingly
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT FROM information_schema.tables 
                        WHERE table_schema = 'farms_manager' 
                        AND table_name = 'expense_advance_permission'
                    ) THEN
                        -- Table exists, drop old constraint and registry table
                        ALTER TABLE farms_manager.expense_advance_permission 
                            DROP CONSTRAINT IF EXISTS fk_expense_advance_permission_expense_advance_registry_entity_;
                        
                        DROP TABLE IF EXISTS farms_manager.expense_advance_registry;
                        
                        -- Rename column if it exists
                        IF EXISTS (
                            SELECT FROM information_schema.columns 
                            WHERE table_schema = 'farms_manager' 
                            AND table_name = 'expense_advance_permission' 
                            AND column_name = 'expense_advance_registry_id'
                        ) THEN
                            ALTER TABLE farms_manager.expense_advance_permission 
                                RENAME COLUMN expense_advance_registry_id TO employee_id;
                        END IF;
                        
                        -- Rename indexes if they exist
                        ALTER INDEX IF EXISTS ""IX_ExpenseAdvancePermissions_UserRegistryType"" 
                            RENAME TO ""IX_ExpenseAdvancePermissions_UserEmployeeType"";
                        ALTER INDEX IF EXISTS ""IX_ExpenseAdvancePermissions_RegistryId"" 
                            RENAME TO ""IX_ExpenseAdvancePermissions_EmployeeId"";
                    ELSE
                        -- Table doesn't exist, create it
                        CREATE TABLE farms_manager.expense_advance_permission (
                            id uuid NOT NULL,
                            created_by uuid NULL,
                            date_created_utc timestamp with time zone NOT NULL,
                            date_deleted_utc timestamp with time zone NULL,
                            date_modified_utc timestamp with time zone NULL,
                            deleted_by uuid NULL,
                            employee_id uuid NOT NULL,
                            modified_by uuid NULL,
                            permission_type character varying(100) NOT NULL,
                            user_id uuid NOT NULL,
                            CONSTRAINT pk_expense_advance_permission PRIMARY KEY (id)
                        );
                        
                        CREATE INDEX ""IX_ExpenseAdvancePermissions_EmployeeId"" 
                            ON farms_manager.expense_advance_permission (employee_id);
                        CREATE UNIQUE INDEX ""IX_ExpenseAdvancePermissions_UserEmployeeType"" 
                            ON farms_manager.expense_advance_permission (user_id, employee_id, permission_type);
                    END IF;
                    
                    -- Add foreign key constraint (will be created in both cases)
                    ALTER TABLE farms_manager.expense_advance_permission 
                        DROP CONSTRAINT IF EXISTS fk_expense_advance_permission_employee_employee_id;
                    ALTER TABLE farms_manager.expense_advance_permission 
                        ADD CONSTRAINT fk_expense_advance_permission_employee_employee_id 
                        FOREIGN KEY (employee_id) 
                        REFERENCES farms_manager.employee(id) 
                        ON DELETE CASCADE;
                END $$;
            ");
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
