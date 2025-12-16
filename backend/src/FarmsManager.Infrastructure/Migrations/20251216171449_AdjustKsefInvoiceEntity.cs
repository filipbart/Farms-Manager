using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustKsefInvoiceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_k_se_f_invoice_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "k_se_f_invoice");

            migrationBuilder.AlterColumn<Guid>(
                name: "assigned_user_id",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_k_se_f_invoice_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "assigned_user_id",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_k_se_f_invoice_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "k_se_f_invoice");

            migrationBuilder.AlterColumn<Guid>(
                name: "assigned_user_id",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_k_se_f_invoice_user_entity_assigned_user_id",
                schema: "farms_manager",
                table: "k_se_f_invoice",
                column: "assigned_user_id",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
