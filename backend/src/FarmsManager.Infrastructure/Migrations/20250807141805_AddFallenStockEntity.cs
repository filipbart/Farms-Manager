using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFallenStockEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fallen_stock",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    utilization_plant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    henhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    date_irz_sent_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sent_to_irz_by = table.Column<Guid>(type: "uuid", nullable: true),
                    document_number = table.Column<string>(type: "text", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fallen_stock", x => x.id);
                    table.ForeignKey(
                        name: "fk_fallen_stock_cycle_entity_cycle_id",
                        column: x => x.cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_fallen_stock_farm_entity_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_fallen_stock_henhouse_entity_henhouse_id",
                        column: x => x.henhouse_id,
                        principalSchema: "farms_manager",
                        principalTable: "henhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_fallen_stock_utilization_plant_entity_utilization_plant_id",
                        column: x => x.utilization_plant_id,
                        principalSchema: "farms_manager",
                        principalTable: "utilization_plant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_cycle_id_farm_id_henhouse_id_utilization_plant",
                schema: "farms_manager",
                table: "fallen_stock",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id", "utilization_plant_id" },
                unique: true,
                filter: "date_deleted_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_farm_id",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_henhouse_id",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "henhouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_utilization_plant_id",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "utilization_plant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fallen_stock",
                schema: "farms_manager");
        }
    }
}
