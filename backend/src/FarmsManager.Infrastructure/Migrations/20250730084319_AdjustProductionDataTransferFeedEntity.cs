using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustProductionDataTransferFeedEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "remaining_value",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "remaining_tonnage",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                newName: "tonnage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "value",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                newName: "remaining_value");

            migrationBuilder.RenameColumn(
                name: "tonnage",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                newName: "remaining_tonnage");
        }
    }
}
