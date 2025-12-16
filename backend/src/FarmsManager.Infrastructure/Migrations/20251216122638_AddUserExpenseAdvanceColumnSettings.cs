using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserExpenseAdvanceColumnSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_expense_advance_column_settings",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    visible_columns = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_expense_advance_column_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_expense_advance_column_settings_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_expense_advance_column_settings_user_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_expense_advance_column_settings_user_entity_user_id",
                        column: x => x.user_id,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_expense_advance_column_settings_user_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_expense_advance_column_settings_created_by",
                schema: "farms_manager",
                table: "user_expense_advance_column_settings",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_expense_advance_column_settings_deleted_by",
                schema: "farms_manager",
                table: "user_expense_advance_column_settings",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_expense_advance_column_settings_modified_by",
                schema: "farms_manager",
                table: "user_expense_advance_column_settings",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "IX_UserExpenseAdvanceColumnSettings_UserId",
                schema: "farms_manager",
                table: "user_expense_advance_column_settings",
                column: "user_id",
                unique: true,
                filter: "date_deleted_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_expense_advance_column_settings",
                schema: "farms_manager");
        }
    }
}
