using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustExpenseAdvanceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_expense_advance_category_expense_advance_ca",
                schema: "farms_manager",
                table: "expense_advance");

            migrationBuilder.AlterColumn<Guid>(
                name: "expense_advance_category_id",
                schema: "farms_manager",
                table: "expense_advance",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_expense_advance_category_expense_advance_ca",
                schema: "farms_manager",
                table: "expense_advance",
                column: "expense_advance_category_id",
                principalSchema: "farms_manager",
                principalTable: "expense_advance_category",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_expense_advance_category_expense_advance_ca",
                schema: "farms_manager",
                table: "expense_advance");

            migrationBuilder.AlterColumn<Guid>(
                name: "expense_advance_category_id",
                schema: "farms_manager",
                table: "expense_advance",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_expense_advance_category_expense_advance_ca",
                schema: "farms_manager",
                table: "expense_advance",
                column: "expense_advance_category_id",
                principalSchema: "farms_manager",
                principalTable: "expense_advance_category",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
