using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCycleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "active_cycle_id",
                schema: "farms_manager",
                table: "farm",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cycle",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identifier = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cycle", x => x.id);
                    table.ForeignKey(
                        name: "fk_cycle_farm_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_farm_active_cycle_id",
                schema: "farms_manager",
                table: "farm",
                column: "active_cycle_id");

            migrationBuilder.CreateIndex(
                name: "ix_cycle_farm_id",
                schema: "farms_manager",
                table: "cycle",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_cycle_identifier_year_farm_id",
                schema: "farms_manager",
                table: "cycle",
                columns: new[] { "identifier", "year", "farm_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_farm_cycle_active_cycle_id",
                schema: "farms_manager",
                table: "farm",
                column: "active_cycle_id",
                principalSchema: "farms_manager",
                principalTable: "cycle",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_farm_cycle_active_cycle_id",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropTable(
                name: "cycle",
                schema: "farms_manager");

            migrationBuilder.DropIndex(
                name: "ix_farm_active_cycle_id",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropColumn(
                name: "active_cycle_id",
                schema: "farms_manager",
                table: "farm");
        }
    }
}
