using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustHatcheryPriceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_hatchery_price_hatchery_id_price_date",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_price_hatchery_id_date",
                schema: "farms_manager",
                table: "hatchery_price",
                columns: new[] { "hatchery_id", "date" },
                unique: true,
                filter: "date_deleted_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_hatchery_price_hatchery_id_date",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_price_hatchery_id_price_date",
                schema: "farms_manager",
                table: "hatchery_price",
                columns: new[] { "hatchery_id", "price", "date" },
                unique: true,
                filter: "date_deleted_utc IS NULL");
        }
    }
}
