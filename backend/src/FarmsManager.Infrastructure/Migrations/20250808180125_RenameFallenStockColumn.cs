using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameFallenStockColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "internal_irz_group_id",
                schema: "farms_manager",
                table: "fallen_stock",
                newName: "internal_group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "internal_group_id",
                schema: "farms_manager",
                table: "fallen_stock",
                newName: "internal_irz_group_id");
        }
    }
}
