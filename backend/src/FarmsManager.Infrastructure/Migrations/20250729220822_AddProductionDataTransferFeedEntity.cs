using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionDataTransferFeedEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "production_data_transfer_feed",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_henhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_henhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("pk_production_data_transfer_feed", x => x.id);
                    table.ForeignKey(
                        name: "fk_production_data_transfer_feed_cycle_from_cycle_id",
                        column: x => x.from_cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_transfer_feed_cycle_to_cycle_id",
                        column: x => x.to_cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_transfer_feed_farm_from_farm_id",
                        column: x => x.from_farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_transfer_feed_farm_to_farm_id",
                        column: x => x.to_farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_transfer_feed_henhouse_from_henhouse_id",
                        column: x => x.from_henhouse_id,
                        principalSchema: "farms_manager",
                        principalTable: "henhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_production_data_transfer_feed_henhouse_to_henhouse_id",
                        column: x => x.to_henhouse_id,
                        principalSchema: "farms_manager",
                        principalTable: "henhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_production_data_transfer_feed_from_cycle_id",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "from_cycle_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_transfer_feed_from_farm_id",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "from_farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_transfer_feed_from_henhouse_id",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "from_henhouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_transfer_feed_to_cycle_id",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "to_cycle_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_transfer_feed_to_farm_id",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "to_farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_transfer_feed_to_henhouse_id",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "to_henhouse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "production_data_transfer_feed",
                schema: "farms_manager");
        }
    }
}
