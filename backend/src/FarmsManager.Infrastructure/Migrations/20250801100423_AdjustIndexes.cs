using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_production_data_weighing_cycle_id_farm_id_henhouse_id_hatch",
                schema: "farms_manager",
                table: "production_data_weighing");

            migrationBuilder.DropIndex(
                name: "ix_production_data_failure_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "production_data_failure");

            migrationBuilder.DropIndex(
                name: "ix_insertion_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropIndex(
                name: "ix_cycle_identifier_year_farm_id",
                schema: "farms_manager",
                table: "cycle");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weighing_cycle_id_farm_id_henhouse_id_hatch",
                schema: "farms_manager",
                table: "production_data_weighing",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id", "hatchery_id" },
                unique: true,
                filter: "date_deleted_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_failure_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "production_data_failure",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id" },
                unique: true,
                filter: "date_deleted_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_insertion_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "insertion",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id" },
                unique: true,
                filter: "date_deleted_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_cycle_identifier_year_farm_id",
                schema: "farms_manager",
                table: "cycle",
                columns: new[] { "identifier", "year", "farm_id" },
                unique: true,
                filter: "date_deleted_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_production_data_weighing_cycle_id_farm_id_henhouse_id_hatch",
                schema: "farms_manager",
                table: "production_data_weighing");

            migrationBuilder.DropIndex(
                name: "ix_production_data_failure_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "production_data_failure");

            migrationBuilder.DropIndex(
                name: "ix_insertion_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropIndex(
                name: "ix_cycle_identifier_year_farm_id",
                schema: "farms_manager",
                table: "cycle");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weighing_cycle_id_farm_id_henhouse_id_hatch",
                schema: "farms_manager",
                table: "production_data_weighing",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id", "hatchery_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_production_data_failure_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "production_data_failure",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_insertion_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "insertion",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cycle_identifier_year_farm_id",
                schema: "farms_manager",
                table: "cycle",
                columns: new[] { "identifier", "year", "farm_id" },
                unique: true);
        }
    }
}
