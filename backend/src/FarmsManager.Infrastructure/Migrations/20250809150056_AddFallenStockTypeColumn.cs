using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFallenStockTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_fallen_stock_utilization_plant_entity_utilization_plant_id",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.AlterColumn<Guid>(
                name: "utilization_plant_id",
                schema: "farms_manager",
                table: "fallen_stock",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "type",
                schema: "farms_manager",
                table: "fallen_stock",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "FallCollision");

            migrationBuilder.AddForeignKey(
                name: "fk_fallen_stock_utilization_plant_entity_utilization_plant_id",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "utilization_plant_id",
                principalSchema: "farms_manager",
                principalTable: "utilization_plant",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_fallen_stock_utilization_plant_entity_utilization_plant_id",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.DropColumn(
                name: "type",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.AlterColumn<Guid>(
                name: "utilization_plant_id",
                schema: "farms_manager",
                table: "fallen_stock",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_fallen_stock_utilization_plant_entity_utilization_plant_id",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "utilization_plant_id",
                principalSchema: "farms_manager",
                principalTable: "utilization_plant",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
