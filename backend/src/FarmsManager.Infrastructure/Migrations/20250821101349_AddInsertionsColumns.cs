using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInsertionsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "reported_to_wios",
                schema: "farms_manager",
                table: "insertion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "wios_comment",
                schema: "farms_manager",
                table: "insertion",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reported_to_wios",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropColumn(
                name: "wios_comment",
                schema: "farms_manager",
                table: "insertion");
        }
    }
}
