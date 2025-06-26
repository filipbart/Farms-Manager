using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProducerNumberColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "producer_number",
                schema: "farms_manager",
                table: "hatchery",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "producer_number",
                schema: "farms_manager",
                table: "farm",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "producer_number",
                schema: "farms_manager",
                table: "hatchery");

            migrationBuilder.DropColumn(
                name: "producer_number",
                schema: "farms_manager",
                table: "farm");
        }
    }
}
