using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionDataWeighingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "production_data_weighing",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    henhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hatchery_id = table.Column<Guid>(type: "uuid", nullable: false),
                    weighing1day = table.Column<int>(type: "integer", nullable: true),
                    weighing1weight = table.Column<decimal>(type: "numeric", nullable: true),
                    weighing2day = table.Column<int>(type: "integer", nullable: true),
                    weighing2weight = table.Column<decimal>(type: "numeric", nullable: true),
                    weighing3day = table.Column<int>(type: "integer", nullable: true),
                    weighing3weight = table.Column<decimal>(type: "numeric", nullable: true),
                    weighing4day = table.Column<int>(type: "integer", nullable: true),
                    weighing4weight = table.Column<decimal>(type: "numeric", nullable: true),
                    weighing5day = table.Column<int>(type: "integer", nullable: true),
                    weighing5weight = table.Column<decimal>(type: "numeric", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_production_data_weighing", x => x.id);
                    table.ForeignKey(
                        name: "fk_production_data_weighing_cycle_cycle_id",
                        column: x => x.cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_weighing_farm_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_weighing_hatchery_hatchery_id",
                        column: x => x.hatchery_id,
                        principalSchema: "farms_manager",
                        principalTable: "hatchery",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_weighing_henhouse_henhouse_id",
                        column: x => x.henhouse_id,
                        principalSchema: "farms_manager",
                        principalTable: "henhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weighing_cycle_id_farm_id_henhouse_id_hatch",
                schema: "farms_manager",
                table: "production_data_weighing",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id", "hatchery_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weighing_farm_id",
                schema: "farms_manager",
                table: "production_data_weighing",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weighing_hatchery_id",
                schema: "farms_manager",
                table: "production_data_weighing",
                column: "hatchery_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weighing_henhouse_id",
                schema: "farms_manager",
                table: "production_data_weighing",
                column: "henhouse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "production_data_weighing",
                schema: "farms_manager");
        }
    }
}
