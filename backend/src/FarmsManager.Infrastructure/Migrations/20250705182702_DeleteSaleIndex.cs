using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSaleIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sale_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "sale");

            migrationBuilder.CreateIndex(
                name: "ix_sale_cycle_id",
                schema: "farms_manager",
                table: "sale",
                column: "cycle_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sale_cycle_id",
                schema: "farms_manager",
                table: "sale");

            migrationBuilder.CreateIndex(
                name: "ix_sale_cycle_id_farm_id_henhouse_id",
                schema: "farms_manager",
                table: "sale",
                columns: new[] { "cycle_id", "farm_id", "henhouse_id" },
                unique: true);
        }
    }
}
