using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFlockLossMeasureEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "production_data_flock_loss_measure",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    henhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hatchery_id = table.Column<Guid>(type: "uuid", nullable: false),
                    insertion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    flock_loss1day = table.Column<int>(type: "integer", nullable: true),
                    flock_loss1quantity = table.Column<int>(type: "integer", nullable: true),
                    flock_loss1percentage = table.Column<decimal>(type: "numeric", nullable: true),
                    flock_loss2day = table.Column<int>(type: "integer", nullable: true),
                    flock_loss2quantity = table.Column<int>(type: "integer", nullable: true),
                    flock_loss2percentage = table.Column<decimal>(type: "numeric", nullable: true),
                    flock_loss3day = table.Column<int>(type: "integer", nullable: true),
                    flock_loss3quantity = table.Column<int>(type: "integer", nullable: true),
                    flock_loss3percentage = table.Column<decimal>(type: "numeric", nullable: true),
                    flock_loss4day = table.Column<int>(type: "integer", nullable: true),
                    flock_loss4quantity = table.Column<int>(type: "integer", nullable: true),
                    flock_loss4percentage = table.Column<decimal>(type: "numeric", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_production_data_flock_loss_measure", x => x.id);
                    table.ForeignKey(
                        name: "fk_production_data_flock_loss_measure_cycle_cycle_id",
                        column: x => x.cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_flock_loss_measure_farm_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_flock_loss_measure_hatchery_hatchery_id",
                        column: x => x.hatchery_id,
                        principalSchema: "farms_manager",
                        principalTable: "hatchery",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_flock_loss_measure_henhouse_henhouse_id",
                        column: x => x.henhouse_id,
                        principalSchema: "farms_manager",
                        principalTable: "henhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_flock_loss_measure_insertion_insertion_id",
                        column: x => x.insertion_id,
                        principalSchema: "farms_manager",
                        principalTable: "insertion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_production_data_flock_loss_measure_cycle_id_farm_id_henhous",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id", "hatchery_id" },
                unique: true,
                filter: "date_deleted_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_flock_loss_measure_farm_id",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_flock_loss_measure_hatchery_id",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                column: "hatchery_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_flock_loss_measure_henhouse_id",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                column: "henhouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_flock_loss_measure_insertion_id",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                column: "insertion_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "production_data_flock_loss_measure",
                schema: "farms_manager");
        }
    }
}
