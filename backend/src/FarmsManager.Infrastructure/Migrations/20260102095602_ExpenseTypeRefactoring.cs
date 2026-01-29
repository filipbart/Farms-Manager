using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseTypeRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expense_contractor_expense_type_entity_expense_type_id",
                schema: "farms_manager",
                table: "expense_contractor");

            migrationBuilder.DropIndex(
                name: "ix_expense_contractor_expense_type_id",
                schema: "farms_manager",
                table: "expense_contractor");

            migrationBuilder.DropColumn(
                name: "expense_type_id",
                schema: "farms_manager",
                table: "expense_contractor");

            migrationBuilder.AddColumn<Guid>(
                name: "expense_type_id",
                schema: "farms_manager",
                table: "expense_production",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "expense_contractor_expense_type",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    expense_contractor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expense_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expense_contractor_expense_type", x => x.id);
                    table.ForeignKey(
                        name: "fk_expense_contractor_expense_type_expense_contractor_expense_",
                        column: x => x.expense_contractor_id,
                        principalSchema: "farms_manager",
                        principalTable: "expense_contractor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_expense_contractor_expense_type_expense_type_entity_expense",
                        column: x => x.expense_type_id,
                        principalSchema: "farms_manager",
                        principalTable: "expense_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_expense_contractor_expense_type_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_expense_contractor_expense_type_user_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_expense_contractor_expense_type_user_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_expense_production_expense_type_id",
                schema: "farms_manager",
                table: "expense_production",
                column: "expense_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_expense_contractor_expense_type_created_by",
                schema: "farms_manager",
                table: "expense_contractor_expense_type",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_contractor_expense_type_deleted_by",
                schema: "farms_manager",
                table: "expense_contractor_expense_type",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_contractor_expense_type_expense_contractor_id_expen",
                schema: "farms_manager",
                table: "expense_contractor_expense_type",
                columns: new[] { "expense_contractor_id", "expense_type_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_expense_contractor_expense_type_expense_type_id",
                schema: "farms_manager",
                table: "expense_contractor_expense_type",
                column: "expense_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_expense_contractor_expense_type_modified_by",
                schema: "farms_manager",
                table: "expense_contractor_expense_type",
                column: "modified_by");

            migrationBuilder.AddForeignKey(
                name: "fk_expense_production_expense_type_entity_expense_type_id",
                schema: "farms_manager",
                table: "expense_production",
                column: "expense_type_id",
                principalSchema: "farms_manager",
                principalTable: "expense_type",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expense_production_expense_type_entity_expense_type_id",
                schema: "farms_manager",
                table: "expense_production");

            migrationBuilder.DropTable(
                name: "expense_contractor_expense_type",
                schema: "farms_manager");

            migrationBuilder.DropIndex(
                name: "ix_expense_production_expense_type_id",
                schema: "farms_manager",
                table: "expense_production");

            migrationBuilder.DropColumn(
                name: "expense_type_id",
                schema: "farms_manager",
                table: "expense_production");

            migrationBuilder.AddColumn<Guid>(
                name: "expense_type_id",
                schema: "farms_manager",
                table: "expense_contractor",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_expense_contractor_expense_type_id",
                schema: "farms_manager",
                table: "expense_contractor",
                column: "expense_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_expense_contractor_expense_type_entity_expense_type_id",
                schema: "farms_manager",
                table: "expense_contractor",
                column: "expense_type_id",
                principalSchema: "farms_manager",
                principalTable: "expense_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
