using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sale",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sale_date = table.Column<DateOnly>(type: "date", nullable: false),
                    slaughterhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    henhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    weight = table.Column<decimal>(type: "numeric", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    confiscated_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    confiscated_count = table.Column<int>(type: "integer", nullable: false),
                    dead_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    dead_count = table.Column<int>(type: "integer", nullable: false),
                    farmer_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    base_price = table.Column<decimal>(type: "numeric", nullable: false),
                    price_with_extras = table.Column<decimal>(type: "numeric", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    other_extras = table.Column<string>(type: "jsonb", nullable: true),
                    internal_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_irz_sent_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_sent_to_irz = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_sale", x => x.id);
                    table.ForeignKey(
                        name: "fk_sale_cycle_cycle_id",
                        column: x => x.cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sale_farm_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sale_henhouse_henhouse_id",
                        column: x => x.henhouse_id,
                        principalSchema: "farms_manager",
                        principalTable: "henhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sale_slaughterhouse_entity_slaughterhouse_id",
                        column: x => x.slaughterhouse_id,
                        principalSchema: "farms_manager",
                        principalTable: "slaughterhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sale_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "sale",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sale_farm_id",
                schema: "farms_manager",
                table: "sale",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_sale_henhouse_id",
                schema: "farms_manager",
                table: "sale",
                column: "henhouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_sale_slaughterhouse_id",
                schema: "farms_manager",
                table: "sale",
                column: "slaughterhouse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sale",
                schema: "farms_manager");
        }
    }
}
