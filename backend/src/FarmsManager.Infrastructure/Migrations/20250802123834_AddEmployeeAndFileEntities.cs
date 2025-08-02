using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeAndFileEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "employee",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    position = table.Column<string>(type: "text", nullable: true),
                    contract_type = table.Column<string>(type: "text", nullable: true),
                    salary = table.Column<decimal>(type: "numeric", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_farm_entity_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_file",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_path = table.Column<string>(type: "text", nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_file", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_file_employee_employee_id",
                        column: x => x.employee_id,
                        principalSchema: "farms_manager",
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employee_farm_id",
                schema: "farms_manager",
                table: "employee",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_file_employee_id",
                schema: "farms_manager",
                table: "employee_file",
                column: "employee_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_file",
                schema: "farms_manager");

            migrationBuilder.DropTable(
                name: "employee",
                schema: "farms_manager");
        }
    }
}
