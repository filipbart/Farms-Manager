using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustFallenStockIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_fallen_stock_cycle_id_farm_id_henhouse_id_utilization_plant",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_cycle_id_farm_id_henhouse_id_date",
                schema: "farms_manager",
                table: "fallen_stock",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id", "date" },
                unique: true,
                filter: "date_deleted_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_fallen_stock_cycle_id_farm_id_henhouse_id_date",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_cycle_id_farm_id_henhouse_id_utilization_plant",
                schema: "farms_manager",
                table: "fallen_stock",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id", "utilization_plant_id" },
                unique: true,
                filter: "date_deleted_utc IS NULL");
        }
    }
}
