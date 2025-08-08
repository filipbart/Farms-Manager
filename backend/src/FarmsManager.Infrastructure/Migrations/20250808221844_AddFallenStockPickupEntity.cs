using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFallenStockPickupEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fallen_stock_pickup",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fallen_stock_pickup", x => x.id);
                    table.ForeignKey(
                        name: "fk_fallen_stock_pickup_cycle_entity_cycle_id",
                        column: x => x.cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_fallen_stock_pickup_farm_entity_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_pickup_cycle_id_farm_id_date",
                schema: "farms_manager",
                table: "fallen_stock_pickup",
                columns: new[] { "cycle_id", "farm_id", "date" },
                unique: true,
                filter: "date_deleted_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_pickup_farm_id",
                schema: "farms_manager",
                table: "fallen_stock_pickup",
                column: "farm_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fallen_stock_pickup",
                schema: "farms_manager");
        }
    }
}
