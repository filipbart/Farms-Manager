using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKSeFSynchronizationLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "k_se_f_synchronization_log",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    invoices_downloaded = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    invoices_saved = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    errors_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_manual = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    duration_seconds = table.Column<double>(type: "double precision", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_k_se_f_synchronization_log", x => x.id);
                    table.ForeignKey(
                        name: "fk_k_se_f_synchronization_log_user_entity_created_by",
                        column: x => x.created_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_k_se_f_synchronization_log_user_entity_deleted_by",
                        column: x => x.deleted_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_k_se_f_synchronization_log_user_entity_modified_by",
                        column: x => x.modified_by,
                        principalSchema: "farms_manager",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_synchronization_log_created_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_synchronization_log_deleted_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_synchronization_log_modified_by",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_synchronization_log_started_at",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                column: "started_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_k_se_f_synchronization_log_status",
                schema: "farms_manager",
                table: "k_se_f_synchronization_log",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "k_se_f_synchronization_log",
                schema: "farms_manager");
        }
    }
}
