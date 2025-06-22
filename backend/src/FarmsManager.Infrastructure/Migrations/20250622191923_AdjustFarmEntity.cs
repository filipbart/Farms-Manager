using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustFarmEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_farm_cycle_active_cycle_id",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.AddForeignKey(
                name: "fk_farm_cycle_active_cycle_id",
                schema: "farms_manager",
                table: "farm",
                column: "active_cycle_id",
                principalSchema: "farms_manager",
                principalTable: "cycle",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_farm_cycle_active_cycle_id",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.AddForeignKey(
                name: "fk_farm_cycle_active_cycle_id",
                schema: "farms_manager",
                table: "farm",
                column: "active_cycle_id",
                principalSchema: "farms_manager",
                principalTable: "cycle",
                principalColumn: "id");
        }
    }
}
