using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseAdvancePermissionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "expense_advance_registry",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_expense_advance_registry", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "expense_advance_permission",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expense_advance_registry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expense_advance_permission", x => x.id);
                    table.ForeignKey(
                        name: "fk_expense_advance_permission_expense_advance_registry_entity_",
                        column: x => x.expense_advance_registry_id,
                        principalSchema: "farms_manager",
                        principalTable: "expense_advance_registry",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_expense_advance_permission_user_entity_user_id",
                        column: x => x.user_id,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAdvancePermissions_RegistryId",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "expense_advance_registry_id");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAdvancePermissions_UserId",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAdvancePermissions_UserRegistryType",
                schema: "farms_manager",
                table: "expense_advance_permission",
                columns: new[] { "user_id", "expense_advance_registry_id", "permission_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expense_advance_permission",
                schema: "farms_manager");

            migrationBuilder.DropTable(
                name: "expense_advance_registry",
                schema: "farms_manager");
        }
    }
}
