using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameKSeFTokenColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "k_se_f_token",
                schema: "farms_manager",
                table: "tax_business_entity",
                newName: "ksef_token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ksef_token",
                schema: "farms_manager",
                table: "tax_business_entity",
                newName: "k_se_f_token");
        }
    }
}
