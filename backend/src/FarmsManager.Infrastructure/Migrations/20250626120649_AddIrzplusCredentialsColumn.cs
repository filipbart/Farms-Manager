using FarmsManager.Domain.Aggregates.UserAggregate.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIrzplusCredentialsColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<IrzplusCredentials>(
                name: "irzplus_credentials",
                schema: "farms_manager",
                table: "user",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "irzplus_credentials",
                schema: "farms_manager",
                table: "user");
        }
    }
}
