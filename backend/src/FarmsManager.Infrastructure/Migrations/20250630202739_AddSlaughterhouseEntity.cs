using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaughterhouseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "slaughterhouse",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nip = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    producer_number = table.Column<string>(type: "text", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_slaughterhouse", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "slaughterhouse",
                schema: "farms_manager");
        }
    }
}
