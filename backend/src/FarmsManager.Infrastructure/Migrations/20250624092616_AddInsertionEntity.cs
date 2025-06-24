using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInsertionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "insertion",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    insertion_date = table.Column<DateOnly>(type: "date", nullable: false),
                    henhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hatchery_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    body_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_insertion", x => x.id);
                    table.ForeignKey(
                        name: "fk_insertion_cycle_cycle_id",
                        column: x => x.cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_insertion_henhouse_henhouse_id",
                        column: x => x.henhouse_id,
                        principalSchema: "farms_manager",
                        principalTable: "henhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_insertion_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "insertion",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_insertion_henhouse_id",
                schema: "farms_manager",
                table: "insertion",
                column: "henhouse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "insertion",
                schema: "farms_manager");
        }
    }
}
