using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIrzCommentColumnForInsertionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "irz_comment",
                schema: "farms_manager",
                table: "insertion",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "irz_comment",
                schema: "farms_manager",
                table: "insertion");
        }
    }
}
