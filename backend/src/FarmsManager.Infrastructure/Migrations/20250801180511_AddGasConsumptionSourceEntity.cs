using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGasConsumptionSourceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "used_quantity",
                schema: "farms_manager",
                table: "gas_delivery",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "cancelled_at_utc",
                schema: "farms_manager",
                table: "gas_consumption",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "correction_for_id",
                schema: "farms_manager",
                table: "gas_consumption",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "farms_manager",
                table: "gas_consumption",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "gas_consumption_source",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    gas_consumption_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gas_delivery_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consumed_quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gas_consumption_source", x => x.id);
                    table.ForeignKey(
                        name: "fk_gas_consumption_source_gas_consumption_gas_consumption_id",
                        column: x => x.gas_consumption_id,
                        principalSchema: "farms_manager",
                        principalTable: "gas_consumption",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_gas_consumption_source_gas_delivery_entity_gas_delivery_id",
                        column: x => x.gas_delivery_id,
                        principalSchema: "farms_manager",
                        principalTable: "gas_delivery",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_gas_consumption_source_gas_consumption_id",
                schema: "farms_manager",
                table: "gas_consumption_source",
                column: "gas_consumption_id");

            migrationBuilder.CreateIndex(
                name: "ix_gas_consumption_source_gas_delivery_id",
                schema: "farms_manager",
                table: "gas_consumption_source",
                column: "gas_delivery_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gas_consumption_source",
                schema: "farms_manager");

            migrationBuilder.DropColumn(
                name: "used_quantity",
                schema: "farms_manager",
                table: "gas_delivery");

            migrationBuilder.DropColumn(
                name: "cancelled_at_utc",
                schema: "farms_manager",
                table: "gas_consumption");

            migrationBuilder.DropColumn(
                name: "correction_for_id",
                schema: "farms_manager",
                table: "gas_consumption");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "farms_manager",
                table: "gas_consumption");
        }
    }
}
