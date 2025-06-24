using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustInsertionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_insertion_farm_id",
                schema: "farms_manager",
                table: "insertion",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_insertion_hatchery_id",
                schema: "farms_manager",
                table: "insertion",
                column: "hatchery_id");

            migrationBuilder.AddForeignKey(
                name: "fk_insertion_farm_farm_id",
                schema: "farms_manager",
                table: "insertion",
                column: "farm_id",
                principalSchema: "farms_manager",
                principalTable: "farm",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_insertion_hatchery_entity_hatchery_id",
                schema: "farms_manager",
                table: "insertion",
                column: "hatchery_id",
                principalSchema: "farms_manager",
                principalTable: "hatchery",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_insertion_farm_farm_id",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropForeignKey(
                name: "fk_insertion_hatchery_entity_hatchery_id",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropIndex(
                name: "ix_insertion_farm_id",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropIndex(
                name: "ix_insertion_hatchery_id",
                schema: "farms_manager",
                table: "insertion");
        }
    }
}
