using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentColumnToInsertionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "comment",
                schema: "farms_manager",
                table: "insertion",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comment",
                schema: "farms_manager",
                table: "insertion");
        }
    }
}
