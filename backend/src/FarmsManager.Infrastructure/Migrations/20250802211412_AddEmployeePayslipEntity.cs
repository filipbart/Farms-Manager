using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeePayslipEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "employee_payslip",
                schema: "farms_manager",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cycle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payroll_period = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    base_salary = table.Column<decimal>(type: "numeric", nullable: false),
                    bank_transfer_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    bonus_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    overtime_pay = table.Column<decimal>(type: "numeric", nullable: false),
                    overtime_hours = table.Column<decimal>(type: "numeric", nullable: false),
                    deductions = table.Column<decimal>(type: "numeric", nullable: false),
                    other_allowances = table.Column<decimal>(type: "numeric", nullable: false),
                    net_pay = table.Column<decimal>(type: "numeric", nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_modified_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_payslip", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_payslip_cycle_entity_cycle_id",
                        column: x => x.cycle_id,
                        principalSchema: "farms_manager",
                        principalTable: "cycle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_payslip_employee_employee_id",
                        column: x => x.employee_id,
                        principalSchema: "farms_manager",
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_payslip_farm_entity_farm_id",
                        column: x => x.farm_id,
                        principalSchema: "farms_manager",
                        principalTable: "farm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employee_payslip_cycle_id",
                schema: "farms_manager",
                table: "employee_payslip",
                column: "cycle_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_payslip_employee_id",
                schema: "farms_manager",
                table: "employee_payslip",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_payslip_farm_id",
                schema: "farms_manager",
                table: "employee_payslip",
                column: "farm_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_payslip",
                schema: "farms_manager");
        }
    }
}
