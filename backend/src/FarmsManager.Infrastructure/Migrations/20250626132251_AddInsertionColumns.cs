using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInsertionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "date_sent_utc",
                schema: "farms_manager",
                table: "insertion",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_sent",
                schema: "farms_manager",
                table: "insertion",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_sent_utc",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropColumn(
                name: "is_sent",
                schema: "farms_manager",
                table: "insertion");
        }
    }
}
