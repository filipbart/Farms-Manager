using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustInsertionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_sent",
                schema: "farms_manager",
                table: "insertion",
                newName: "is_sent_to_irz");

            migrationBuilder.RenameColumn(
                name: "date_sent_utc",
                schema: "farms_manager",
                table: "insertion",
                newName: "date_irz_sent_utc");

            migrationBuilder.AddColumn<Guid>(
                name: "sent_to_irz_by",
                schema: "farms_manager",
                table: "insertion",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sent_to_irz_by",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.RenameColumn(
                name: "is_sent_to_irz",
                schema: "farms_manager",
                table: "insertion",
                newName: "is_sent");

            migrationBuilder.RenameColumn(
                name: "date_irz_sent_utc",
                schema: "farms_manager",
                table: "insertion",
                newName: "date_sent_utc");
        }
    }
}
