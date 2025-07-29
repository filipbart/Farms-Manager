using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionDataRemainingFeedEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "production_data_remaining_feed",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    henhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_name = table.Column<string>(type: "text", nullable: true),
                    remaining_tonnage = table.Column<int>(type: "integer", nullable: false),
                    remaining_value = table.Column<decimal>(type: "numeric", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_production_data_remaining_feed", x => x.id);
                    table.ForeignKey(
                        name: "fk_production_data_remaining_feed_cycle_cycle_id",
                        column: x => x.cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_remaining_feed_farm_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_remaining_feed_henhouse_henhouse_id",
                        column: x => x.henhouse_id,
                        principalSchema: "farms_manager",
                        principalTable: "henhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_production_data_remaining_feed_cycle_id",
                schema: "farms_manager",
                table: "production_data_remaining_feed",
                column: "cycle_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_remaining_feed_farm_id",
                schema: "farms_manager",
                table: "production_data_remaining_feed",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_remaining_feed_henhouse_id",
                schema: "farms_manager",
                table: "production_data_remaining_feed",
                column: "henhouse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "production_data_remaining_feed",
                schema: "farms_manager");
        }
    }
}
