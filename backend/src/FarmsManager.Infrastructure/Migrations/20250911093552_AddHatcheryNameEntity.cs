using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHatcheryNameEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_price_hatchery_hatchery_id",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_price_hatchery_id_date",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.DropColumn(
                name: "hatchery_id",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.AddColumn<string>(
                name: "hatchery_name",
                schema: "farms_manager",
                table: "hatchery_price",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "hatchery_name",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hatchery_name", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_price_hatchery_name_date",
                schema: "farms_manager",
                table: "hatchery_price",
                columns: new[] { "hatchery_name", "date" },
                unique: true,
                filter: "date_deleted_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hatchery_name",
                schema: "farms_manager");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_price_hatchery_name_date",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.DropColumn(
                name: "hatchery_name",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.AddColumn<Guid>(
                name: "hatchery_id",
                schema: "farms_manager",
                table: "hatchery_price",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_price_hatchery_id_date",
                schema: "farms_manager",
                table: "hatchery_price",
                columns: new[] { "hatchery_id", "date" },
                unique: true,
                filter: "date_deleted_utc IS NULL");

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_price_hatchery_hatchery_id",
                schema: "farms_manager",
                table: "hatchery_price",
                column: "hatchery_id",
                principalSchema: "farms_manager",
                principalTable: "hatchery",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
