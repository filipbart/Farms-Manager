using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxBusinessEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "tax_business_entity_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "tax_business_entity_id",
                schema: "farms_manager",
                table: "farm",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tax_business_entity",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nip = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    business_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tax_business_entity", x => x.id);
                    table.ForeignKey(
                        name: "fk_tax_business_entity_user_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tax_business_entity_user_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tax_business_entity_user_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ksef_invoice_tax_business_entity_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "tax_business_entity_id");

            migrationBuilder.CreateIndex(
                name: "ix_farm_tax_business_entity_id",
                schema: "farms_manager",
                table: "farm",
                column: "tax_business_entity_id");

            migrationBuilder.CreateIndex(
                name: "ix_tax_business_entity_created_by",
                schema: "farms_manager",
                table: "tax_business_entity",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_tax_business_entity_deleted_by",
                schema: "farms_manager",
                table: "tax_business_entity",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_tax_business_entity_modified_by",
                schema: "farms_manager",
                table: "tax_business_entity",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_tax_business_entity_nip",
                schema: "farms_manager",
                table: "tax_business_entity",
                column: "nip");

            migrationBuilder.CreateIndex(
                name: "ix_tax_business_entity_nip_name",
                schema: "farms_manager",
                table: "tax_business_entity",
                columns: new[] { "nip", "name" });

            migrationBuilder.AddForeignKey(
                name: "fk_farm_tax_business_entity_tax_business_entity_id",
                schema: "farms_manager",
                table: "farm",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_ksef_invoice_tax_business_entity_tax_business_entity_id",
                schema: "farms_manager",
                table: "ksef_invoice",
                column: "tax_business_entity_id",
                principalSchema: "farms_manager",
                principalTable: "tax_business_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_farm_tax_business_entity_tax_business_entity_id",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropForeignKey(
                name: "fk_ksef_invoice_tax_business_entity_tax_business_entity_id",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropTable(
                name: "tax_business_entity",
                schema: "farms_manager");

            migrationBuilder.DropIndex(
                name: "ix_ksef_invoice_tax_business_entity_id",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropIndex(
                name: "ix_farm_tax_business_entity_id",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropColumn(
                name: "tax_business_entity_id",
                schema: "farms_manager",
                table: "ksef_invoice");

            migrationBuilder.DropColumn(
                name: "tax_business_entity_id",
                schema: "farms_manager",
                table: "farm");
        }
    }
}
