using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "farms_manager",
                table: "user_farm",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by",
                schema: "farms_manager",
                table: "user_farm",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "modified_by",
                schema: "farms_manager",
                table: "user_farm",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_utilization_plant_created_by",
                schema: "farms_manager",
                table: "utilization_plant",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_utilization_plant_deleted_by",
                schema: "farms_manager",
                table: "utilization_plant",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_utilization_plant_modified_by",
                schema: "farms_manager",
                table: "utilization_plant",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_session_created_by",
                schema: "farms_manager",
                table: "user_session",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_session_deleted_by",
                schema: "farms_manager",
                table: "user_session",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_session_modified_by",
                schema: "farms_manager",
                table: "user_session",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_permission_created_by",
                schema: "farms_manager",
                table: "user_permission",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_permission_deleted_by",
                schema: "farms_manager",
                table: "user_permission",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_permission_modified_by",
                schema: "farms_manager",
                table: "user_permission",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_farm_created_by",
                schema: "farms_manager",
                table: "user_farm",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_farm_deleted_by",
                schema: "farms_manager",
                table: "user_farm",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_farm_modified_by",
                schema: "farms_manager",
                table: "user_farm",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_slaughterhouse_created_by",
                schema: "farms_manager",
                table: "slaughterhouse",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_slaughterhouse_deleted_by",
                schema: "farms_manager",
                table: "slaughterhouse",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_slaughterhouse_modified_by",
                schema: "farms_manager",
                table: "slaughterhouse",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_sale_invoice_created_by",
                schema: "farms_manager",
                table: "sale_invoice",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_sale_invoice_deleted_by",
                schema: "farms_manager",
                table: "sale_invoice",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_sale_invoice_modified_by",
                schema: "farms_manager",
                table: "sale_invoice",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_sale_field_extra_created_by",
                schema: "farms_manager",
                table: "sale_field_extra",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_sale_field_extra_deleted_by",
                schema: "farms_manager",
                table: "sale_field_extra",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_sale_field_extra_modified_by",
                schema: "farms_manager",
                table: "sale_field_extra",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_sale_created_by",
                schema: "farms_manager",
                table: "sale",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_sale_deleted_by",
                schema: "farms_manager",
                table: "sale",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_sale_modified_by",
                schema: "farms_manager",
                table: "sale",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weight_standard_created_by",
                schema: "farms_manager",
                table: "production_data_weight_standard",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weight_standard_deleted_by",
                schema: "farms_manager",
                table: "production_data_weight_standard",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weight_standard_modified_by",
                schema: "farms_manager",
                table: "production_data_weight_standard",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weighing_created_by",
                schema: "farms_manager",
                table: "production_data_weighing",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weighing_deleted_by",
                schema: "farms_manager",
                table: "production_data_weighing",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_weighing_modified_by",
                schema: "farms_manager",
                table: "production_data_weighing",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_transfer_feed_created_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_transfer_feed_deleted_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_transfer_feed_modified_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_remaining_feed_created_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_remaining_feed_deleted_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_remaining_feed_modified_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_flock_loss_measure_created_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_flock_loss_measure_deleted_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_flock_loss_measure_modified_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_failure_created_by",
                schema: "farms_manager",
                table: "production_data_failure",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_failure_deleted_by",
                schema: "farms_manager",
                table: "production_data_failure",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_production_data_failure_modified_by",
                schema: "farms_manager",
                table: "production_data_failure",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_insertion_created_by",
                schema: "farms_manager",
                table: "insertion",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_insertion_deleted_by",
                schema: "farms_manager",
                table: "insertion",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_insertion_modified_by",
                schema: "farms_manager",
                table: "insertion",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_henhouse_created_by",
                schema: "farms_manager",
                table: "henhouse",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_henhouse_deleted_by",
                schema: "farms_manager",
                table: "henhouse",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_henhouse_modified_by",
                schema: "farms_manager",
                table: "henhouse",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_price_created_by",
                schema: "farms_manager",
                table: "hatchery_price",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_price_deleted_by",
                schema: "farms_manager",
                table: "hatchery_price",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_price_modified_by",
                schema: "farms_manager",
                table: "hatchery_price",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_note_created_by",
                schema: "farms_manager",
                table: "hatchery_note",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_note_deleted_by",
                schema: "farms_manager",
                table: "hatchery_note",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_note_modified_by",
                schema: "farms_manager",
                table: "hatchery_note",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_name_created_by",
                schema: "farms_manager",
                table: "hatchery_name",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_name_deleted_by",
                schema: "farms_manager",
                table: "hatchery_name",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_name_modified_by",
                schema: "farms_manager",
                table: "hatchery_name",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_created_by",
                schema: "farms_manager",
                table: "hatchery",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_deleted_by",
                schema: "farms_manager",
                table: "hatchery",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_hatchery_modified_by",
                schema: "farms_manager",
                table: "hatchery",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_delivery_created_by",
                schema: "farms_manager",
                table: "gas_delivery",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_delivery_deleted_by",
                schema: "farms_manager",
                table: "gas_delivery",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_delivery_modified_by",
                schema: "farms_manager",
                table: "gas_delivery",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_contractor_created_by",
                schema: "farms_manager",
                table: "gas_contractor",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_contractor_deleted_by",
                schema: "farms_manager",
                table: "gas_contractor",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_contractor_modified_by",
                schema: "farms_manager",
                table: "gas_contractor",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_consumption_source_created_by",
                schema: "farms_manager",
                table: "gas_consumption_source",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_consumption_source_deleted_by",
                schema: "farms_manager",
                table: "gas_consumption_source",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_consumption_source_modified_by",
                schema: "farms_manager",
                table: "gas_consumption_source",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_consumption_created_by",
                schema: "farms_manager",
                table: "gas_consumption",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_consumption_deleted_by",
                schema: "farms_manager",
                table: "gas_consumption",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_gas_consumption_modified_by",
                schema: "farms_manager",
                table: "gas_consumption",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_price_created_by",
                schema: "farms_manager",
                table: "feed_price",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_price_deleted_by",
                schema: "farms_manager",
                table: "feed_price",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_price_modified_by",
                schema: "farms_manager",
                table: "feed_price",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_payment_created_by",
                schema: "farms_manager",
                table: "feed_payment",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_payment_deleted_by",
                schema: "farms_manager",
                table: "feed_payment",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_payment_modified_by",
                schema: "farms_manager",
                table: "feed_payment",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_name_created_by",
                schema: "farms_manager",
                table: "feed_name",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_name_deleted_by",
                schema: "farms_manager",
                table: "feed_name",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_name_modified_by",
                schema: "farms_manager",
                table: "feed_name",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_invoice_correction_created_by",
                schema: "farms_manager",
                table: "feed_invoice_correction",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_invoice_correction_deleted_by",
                schema: "farms_manager",
                table: "feed_invoice_correction",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_invoice_correction_modified_by",
                schema: "farms_manager",
                table: "feed_invoice_correction",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_invoice_created_by",
                schema: "farms_manager",
                table: "feed_invoice",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_invoice_deleted_by",
                schema: "farms_manager",
                table: "feed_invoice",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_invoice_modified_by",
                schema: "farms_manager",
                table: "feed_invoice",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_contractor_created_by",
                schema: "farms_manager",
                table: "feed_contractor",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_contractor_deleted_by",
                schema: "farms_manager",
                table: "feed_contractor",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_feed_contractor_modified_by",
                schema: "farms_manager",
                table: "feed_contractor",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_farm_created_by",
                schema: "farms_manager",
                table: "farm",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_farm_deleted_by",
                schema: "farms_manager",
                table: "farm",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_farm_modified_by",
                schema: "farms_manager",
                table: "farm",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_pickup_created_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_pickup_deleted_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_pickup_modified_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_created_by",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_deleted_by",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_fallen_stock_modified_by",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_type_created_by",
                schema: "farms_manager",
                table: "expense_type",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_type_deleted_by",
                schema: "farms_manager",
                table: "expense_type",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_type_modified_by",
                schema: "farms_manager",
                table: "expense_type",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_production_created_by",
                schema: "farms_manager",
                table: "expense_production",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_production_deleted_by",
                schema: "farms_manager",
                table: "expense_production",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_production_modified_by",
                schema: "farms_manager",
                table: "expense_production",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_contractor_created_by",
                schema: "farms_manager",
                table: "expense_contractor",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_contractor_deleted_by",
                schema: "farms_manager",
                table: "expense_contractor",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_contractor_modified_by",
                schema: "farms_manager",
                table: "expense_contractor",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_advance_permission_created_by",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_advance_permission_deleted_by",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_advance_permission_modified_by",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_advance_category_created_by",
                schema: "farms_manager",
                table: "expense_advance_category",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_advance_category_deleted_by",
                schema: "farms_manager",
                table: "expense_advance_category",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_advance_category_modified_by",
                schema: "farms_manager",
                table: "expense_advance_category",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_advance_created_by",
                schema: "farms_manager",
                table: "expense_advance",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_advance_deleted_by",
                schema: "farms_manager",
                table: "expense_advance",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_expense_advance_modified_by",
                schema: "farms_manager",
                table: "expense_advance",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_reminder_created_by",
                schema: "farms_manager",
                table: "employee_reminder",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_reminder_deleted_by",
                schema: "farms_manager",
                table: "employee_reminder",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_reminder_modified_by",
                schema: "farms_manager",
                table: "employee_reminder",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_payslip_created_by",
                schema: "farms_manager",
                table: "employee_payslip",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_payslip_deleted_by",
                schema: "farms_manager",
                table: "employee_payslip",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_payslip_modified_by",
                schema: "farms_manager",
                table: "employee_payslip",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_file_created_by",
                schema: "farms_manager",
                table: "employee_file",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_file_deleted_by",
                schema: "farms_manager",
                table: "employee_file",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_file_modified_by",
                schema: "farms_manager",
                table: "employee_file",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_created_by",
                schema: "farms_manager",
                table: "employee",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_deleted_by",
                schema: "farms_manager",
                table: "employee",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_employee_modified_by",
                schema: "farms_manager",
                table: "employee",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_cycle_created_by",
                schema: "farms_manager",
                table: "cycle",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_cycle_deleted_by",
                schema: "farms_manager",
                table: "cycle",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_cycle_modified_by",
                schema: "farms_manager",
                table: "cycle",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_column_view_created_by",
                schema: "farms_manager",
                table: "column_view",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_column_view_deleted_by",
                schema: "farms_manager",
                table: "column_view",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "ix_column_view_modified_by",
                schema: "farms_manager",
                table: "column_view",
                column: "modified_by");

            migrationBuilder.AddForeignKey(
                name: "fk_column_view_user_created_by",
                schema: "farms_manager",
                table: "column_view",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_column_view_user_deleted_by",
                schema: "farms_manager",
                table: "column_view",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_column_view_user_modified_by",
                schema: "farms_manager",
                table: "column_view",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_cycle_user_created_by",
                schema: "farms_manager",
                table: "cycle",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_cycle_user_deleted_by",
                schema: "farms_manager",
                table: "cycle",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_cycle_user_modified_by",
                schema: "farms_manager",
                table: "cycle",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_user_created_by",
                schema: "farms_manager",
                table: "employee",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_user_deleted_by",
                schema: "farms_manager",
                table: "employee",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_user_modified_by",
                schema: "farms_manager",
                table: "employee",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_file_user_created_by",
                schema: "farms_manager",
                table: "employee_file",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_file_user_deleted_by",
                schema: "farms_manager",
                table: "employee_file",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_file_user_modified_by",
                schema: "farms_manager",
                table: "employee_file",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_payslip_user_created_by",
                schema: "farms_manager",
                table: "employee_payslip",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_payslip_user_deleted_by",
                schema: "farms_manager",
                table: "employee_payslip",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_payslip_user_modified_by",
                schema: "farms_manager",
                table: "employee_payslip",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_reminder_user_created_by",
                schema: "farms_manager",
                table: "employee_reminder",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_reminder_user_deleted_by",
                schema: "farms_manager",
                table: "employee_reminder",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_reminder_user_modified_by",
                schema: "farms_manager",
                table: "employee_reminder",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_user_created_by",
                schema: "farms_manager",
                table: "expense_advance",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_user_deleted_by",
                schema: "farms_manager",
                table: "expense_advance",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_user_modified_by",
                schema: "farms_manager",
                table: "expense_advance",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_category_user_created_by",
                schema: "farms_manager",
                table: "expense_advance_category",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_category_user_deleted_by",
                schema: "farms_manager",
                table: "expense_advance_category",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_category_user_modified_by",
                schema: "farms_manager",
                table: "expense_advance_category",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_permission_user_created_by",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_permission_user_deleted_by",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_advance_permission_user_modified_by",
                schema: "farms_manager",
                table: "expense_advance_permission",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_contractor_user_created_by",
                schema: "farms_manager",
                table: "expense_contractor",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_contractor_user_deleted_by",
                schema: "farms_manager",
                table: "expense_contractor",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_contractor_user_modified_by",
                schema: "farms_manager",
                table: "expense_contractor",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_production_user_created_by",
                schema: "farms_manager",
                table: "expense_production",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_production_user_deleted_by",
                schema: "farms_manager",
                table: "expense_production",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_production_user_modified_by",
                schema: "farms_manager",
                table: "expense_production",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_type_user_created_by",
                schema: "farms_manager",
                table: "expense_type",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_type_user_deleted_by",
                schema: "farms_manager",
                table: "expense_type",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_expense_type_user_modified_by",
                schema: "farms_manager",
                table: "expense_type",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fallen_stock_user_created_by",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fallen_stock_user_deleted_by",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fallen_stock_user_modified_by",
                schema: "farms_manager",
                table: "fallen_stock",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fallen_stock_pickup_user_created_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fallen_stock_pickup_user_deleted_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fallen_stock_pickup_user_modified_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_farm_user_created_by",
                schema: "farms_manager",
                table: "farm",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_farm_user_deleted_by",
                schema: "farms_manager",
                table: "farm",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_farm_user_modified_by",
                schema: "farms_manager",
                table: "farm",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_contractor_user_created_by",
                schema: "farms_manager",
                table: "feed_contractor",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_contractor_user_deleted_by",
                schema: "farms_manager",
                table: "feed_contractor",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_contractor_user_modified_by",
                schema: "farms_manager",
                table: "feed_contractor",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_invoice_user_created_by",
                schema: "farms_manager",
                table: "feed_invoice",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_invoice_user_deleted_by",
                schema: "farms_manager",
                table: "feed_invoice",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_invoice_user_modified_by",
                schema: "farms_manager",
                table: "feed_invoice",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_invoice_correction_user_created_by",
                schema: "farms_manager",
                table: "feed_invoice_correction",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_invoice_correction_user_deleted_by",
                schema: "farms_manager",
                table: "feed_invoice_correction",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_invoice_correction_user_modified_by",
                schema: "farms_manager",
                table: "feed_invoice_correction",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_name_user_created_by",
                schema: "farms_manager",
                table: "feed_name",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_name_user_deleted_by",
                schema: "farms_manager",
                table: "feed_name",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_name_user_modified_by",
                schema: "farms_manager",
                table: "feed_name",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_payment_user_created_by",
                schema: "farms_manager",
                table: "feed_payment",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_payment_user_deleted_by",
                schema: "farms_manager",
                table: "feed_payment",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_payment_user_modified_by",
                schema: "farms_manager",
                table: "feed_payment",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_price_user_created_by",
                schema: "farms_manager",
                table: "feed_price",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_price_user_deleted_by",
                schema: "farms_manager",
                table: "feed_price",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_feed_price_user_modified_by",
                schema: "farms_manager",
                table: "feed_price",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_consumption_user_created_by",
                schema: "farms_manager",
                table: "gas_consumption",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_consumption_user_deleted_by",
                schema: "farms_manager",
                table: "gas_consumption",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_consumption_user_modified_by",
                schema: "farms_manager",
                table: "gas_consumption",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_consumption_source_user_created_by",
                schema: "farms_manager",
                table: "gas_consumption_source",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_consumption_source_user_deleted_by",
                schema: "farms_manager",
                table: "gas_consumption_source",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_consumption_source_user_modified_by",
                schema: "farms_manager",
                table: "gas_consumption_source",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_contractor_user_created_by",
                schema: "farms_manager",
                table: "gas_contractor",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_contractor_user_deleted_by",
                schema: "farms_manager",
                table: "gas_contractor",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_contractor_user_modified_by",
                schema: "farms_manager",
                table: "gas_contractor",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_delivery_user_created_by",
                schema: "farms_manager",
                table: "gas_delivery",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_delivery_user_deleted_by",
                schema: "farms_manager",
                table: "gas_delivery",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gas_delivery_user_modified_by",
                schema: "farms_manager",
                table: "gas_delivery",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_user_created_by",
                schema: "farms_manager",
                table: "hatchery",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_user_deleted_by",
                schema: "farms_manager",
                table: "hatchery",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_user_modified_by",
                schema: "farms_manager",
                table: "hatchery",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_name_user_created_by",
                schema: "farms_manager",
                table: "hatchery_name",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_name_user_deleted_by",
                schema: "farms_manager",
                table: "hatchery_name",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_name_user_modified_by",
                schema: "farms_manager",
                table: "hatchery_name",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_note_user_created_by",
                schema: "farms_manager",
                table: "hatchery_note",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_note_user_deleted_by",
                schema: "farms_manager",
                table: "hatchery_note",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_note_user_modified_by",
                schema: "farms_manager",
                table: "hatchery_note",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_price_user_created_by",
                schema: "farms_manager",
                table: "hatchery_price",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_price_user_deleted_by",
                schema: "farms_manager",
                table: "hatchery_price",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_hatchery_price_user_modified_by",
                schema: "farms_manager",
                table: "hatchery_price",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_henhouse_user_created_by",
                schema: "farms_manager",
                table: "henhouse",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_henhouse_user_deleted_by",
                schema: "farms_manager",
                table: "henhouse",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_henhouse_user_modified_by",
                schema: "farms_manager",
                table: "henhouse",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_insertion_user_created_by",
                schema: "farms_manager",
                table: "insertion",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_insertion_user_deleted_by",
                schema: "farms_manager",
                table: "insertion",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_insertion_user_modified_by",
                schema: "farms_manager",
                table: "insertion",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_failure_user_created_by",
                schema: "farms_manager",
                table: "production_data_failure",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_failure_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_failure",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_failure_user_modified_by",
                schema: "farms_manager",
                table: "production_data_failure",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_flock_loss_measure_user_created_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_flock_loss_measure_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_flock_loss_measure_user_modified_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_remaining_feed_user_created_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_remaining_feed_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_remaining_feed_user_modified_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_transfer_feed_user_created_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_transfer_feed_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_transfer_feed_user_modified_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_weighing_user_created_by",
                schema: "farms_manager",
                table: "production_data_weighing",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_weighing_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_weighing",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_weighing_user_modified_by",
                schema: "farms_manager",
                table: "production_data_weighing",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_weight_standard_user_created_by",
                schema: "farms_manager",
                table: "production_data_weight_standard",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_weight_standard_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_weight_standard",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_production_data_weight_standard_user_modified_by",
                schema: "farms_manager",
                table: "production_data_weight_standard",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sale_user_created_by",
                schema: "farms_manager",
                table: "sale",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sale_user_deleted_by",
                schema: "farms_manager",
                table: "sale",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sale_user_modified_by",
                schema: "farms_manager",
                table: "sale",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sale_field_extra_user_created_by",
                schema: "farms_manager",
                table: "sale_field_extra",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sale_field_extra_user_deleted_by",
                schema: "farms_manager",
                table: "sale_field_extra",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sale_field_extra_user_modified_by",
                schema: "farms_manager",
                table: "sale_field_extra",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sale_invoice_user_created_by",
                schema: "farms_manager",
                table: "sale_invoice",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sale_invoice_user_deleted_by",
                schema: "farms_manager",
                table: "sale_invoice",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sale_invoice_user_modified_by",
                schema: "farms_manager",
                table: "sale_invoice",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_slaughterhouse_user_created_by",
                schema: "farms_manager",
                table: "slaughterhouse",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_slaughterhouse_user_deleted_by",
                schema: "farms_manager",
                table: "slaughterhouse",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_slaughterhouse_user_modified_by",
                schema: "farms_manager",
                table: "slaughterhouse",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_farm_user_created_by",
                schema: "farms_manager",
                table: "user_farm",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_farm_user_deleted_by",
                schema: "farms_manager",
                table: "user_farm",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_farm_user_modified_by",
                schema: "farms_manager",
                table: "user_farm",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_permission_user_created_by",
                schema: "farms_manager",
                table: "user_permission",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_permission_user_deleted_by",
                schema: "farms_manager",
                table: "user_permission",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_permission_user_modified_by",
                schema: "farms_manager",
                table: "user_permission",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_session_user_created_by",
                schema: "farms_manager",
                table: "user_session",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_session_user_deleted_by",
                schema: "farms_manager",
                table: "user_session",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_session_user_modified_by",
                schema: "farms_manager",
                table: "user_session",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_utilization_plant_user_created_by",
                schema: "farms_manager",
                table: "utilization_plant",
                column: "created_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_utilization_plant_user_deleted_by",
                schema: "farms_manager",
                table: "utilization_plant",
                column: "deleted_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_utilization_plant_user_modified_by",
                schema: "farms_manager",
                table: "utilization_plant",
                column: "modified_by",
                principalSchema: "farms_manager",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_column_view_user_created_by",
                schema: "farms_manager",
                table: "column_view");

            migrationBuilder.DropForeignKey(
                name: "fk_column_view_user_deleted_by",
                schema: "farms_manager",
                table: "column_view");

            migrationBuilder.DropForeignKey(
                name: "fk_column_view_user_modified_by",
                schema: "farms_manager",
                table: "column_view");

            migrationBuilder.DropForeignKey(
                name: "fk_cycle_user_created_by",
                schema: "farms_manager",
                table: "cycle");

            migrationBuilder.DropForeignKey(
                name: "fk_cycle_user_deleted_by",
                schema: "farms_manager",
                table: "cycle");

            migrationBuilder.DropForeignKey(
                name: "fk_cycle_user_modified_by",
                schema: "farms_manager",
                table: "cycle");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_user_created_by",
                schema: "farms_manager",
                table: "employee");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_user_deleted_by",
                schema: "farms_manager",
                table: "employee");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_user_modified_by",
                schema: "farms_manager",
                table: "employee");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_file_user_created_by",
                schema: "farms_manager",
                table: "employee_file");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_file_user_deleted_by",
                schema: "farms_manager",
                table: "employee_file");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_file_user_modified_by",
                schema: "farms_manager",
                table: "employee_file");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_payslip_user_created_by",
                schema: "farms_manager",
                table: "employee_payslip");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_payslip_user_deleted_by",
                schema: "farms_manager",
                table: "employee_payslip");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_payslip_user_modified_by",
                schema: "farms_manager",
                table: "employee_payslip");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_reminder_user_created_by",
                schema: "farms_manager",
                table: "employee_reminder");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_reminder_user_deleted_by",
                schema: "farms_manager",
                table: "employee_reminder");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_reminder_user_modified_by",
                schema: "farms_manager",
                table: "employee_reminder");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_user_created_by",
                schema: "farms_manager",
                table: "expense_advance");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_user_deleted_by",
                schema: "farms_manager",
                table: "expense_advance");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_user_modified_by",
                schema: "farms_manager",
                table: "expense_advance");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_category_user_created_by",
                schema: "farms_manager",
                table: "expense_advance_category");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_category_user_deleted_by",
                schema: "farms_manager",
                table: "expense_advance_category");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_category_user_modified_by",
                schema: "farms_manager",
                table: "expense_advance_category");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_permission_user_created_by",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_permission_user_deleted_by",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_advance_permission_user_modified_by",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_contractor_user_created_by",
                schema: "farms_manager",
                table: "expense_contractor");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_contractor_user_deleted_by",
                schema: "farms_manager",
                table: "expense_contractor");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_contractor_user_modified_by",
                schema: "farms_manager",
                table: "expense_contractor");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_production_user_created_by",
                schema: "farms_manager",
                table: "expense_production");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_production_user_deleted_by",
                schema: "farms_manager",
                table: "expense_production");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_production_user_modified_by",
                schema: "farms_manager",
                table: "expense_production");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_type_user_created_by",
                schema: "farms_manager",
                table: "expense_type");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_type_user_deleted_by",
                schema: "farms_manager",
                table: "expense_type");

            migrationBuilder.DropForeignKey(
                name: "fk_expense_type_user_modified_by",
                schema: "farms_manager",
                table: "expense_type");

            migrationBuilder.DropForeignKey(
                name: "fk_fallen_stock_user_created_by",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.DropForeignKey(
                name: "fk_fallen_stock_user_deleted_by",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.DropForeignKey(
                name: "fk_fallen_stock_user_modified_by",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.DropForeignKey(
                name: "fk_fallen_stock_pickup_user_created_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup");

            migrationBuilder.DropForeignKey(
                name: "fk_fallen_stock_pickup_user_deleted_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup");

            migrationBuilder.DropForeignKey(
                name: "fk_fallen_stock_pickup_user_modified_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup");

            migrationBuilder.DropForeignKey(
                name: "fk_farm_user_created_by",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropForeignKey(
                name: "fk_farm_user_deleted_by",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropForeignKey(
                name: "fk_farm_user_modified_by",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_contractor_user_created_by",
                schema: "farms_manager",
                table: "feed_contractor");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_contractor_user_deleted_by",
                schema: "farms_manager",
                table: "feed_contractor");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_contractor_user_modified_by",
                schema: "farms_manager",
                table: "feed_contractor");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_invoice_user_created_by",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_invoice_user_deleted_by",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_invoice_user_modified_by",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_invoice_correction_user_created_by",
                schema: "farms_manager",
                table: "feed_invoice_correction");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_invoice_correction_user_deleted_by",
                schema: "farms_manager",
                table: "feed_invoice_correction");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_invoice_correction_user_modified_by",
                schema: "farms_manager",
                table: "feed_invoice_correction");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_name_user_created_by",
                schema: "farms_manager",
                table: "feed_name");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_name_user_deleted_by",
                schema: "farms_manager",
                table: "feed_name");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_name_user_modified_by",
                schema: "farms_manager",
                table: "feed_name");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_payment_user_created_by",
                schema: "farms_manager",
                table: "feed_payment");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_payment_user_deleted_by",
                schema: "farms_manager",
                table: "feed_payment");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_payment_user_modified_by",
                schema: "farms_manager",
                table: "feed_payment");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_price_user_created_by",
                schema: "farms_manager",
                table: "feed_price");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_price_user_deleted_by",
                schema: "farms_manager",
                table: "feed_price");

            migrationBuilder.DropForeignKey(
                name: "fk_feed_price_user_modified_by",
                schema: "farms_manager",
                table: "feed_price");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_consumption_user_created_by",
                schema: "farms_manager",
                table: "gas_consumption");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_consumption_user_deleted_by",
                schema: "farms_manager",
                table: "gas_consumption");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_consumption_user_modified_by",
                schema: "farms_manager",
                table: "gas_consumption");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_consumption_source_user_created_by",
                schema: "farms_manager",
                table: "gas_consumption_source");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_consumption_source_user_deleted_by",
                schema: "farms_manager",
                table: "gas_consumption_source");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_consumption_source_user_modified_by",
                schema: "farms_manager",
                table: "gas_consumption_source");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_contractor_user_created_by",
                schema: "farms_manager",
                table: "gas_contractor");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_contractor_user_deleted_by",
                schema: "farms_manager",
                table: "gas_contractor");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_contractor_user_modified_by",
                schema: "farms_manager",
                table: "gas_contractor");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_delivery_user_created_by",
                schema: "farms_manager",
                table: "gas_delivery");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_delivery_user_deleted_by",
                schema: "farms_manager",
                table: "gas_delivery");

            migrationBuilder.DropForeignKey(
                name: "fk_gas_delivery_user_modified_by",
                schema: "farms_manager",
                table: "gas_delivery");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_user_created_by",
                schema: "farms_manager",
                table: "hatchery");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_user_deleted_by",
                schema: "farms_manager",
                table: "hatchery");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_user_modified_by",
                schema: "farms_manager",
                table: "hatchery");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_name_user_created_by",
                schema: "farms_manager",
                table: "hatchery_name");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_name_user_deleted_by",
                schema: "farms_manager",
                table: "hatchery_name");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_name_user_modified_by",
                schema: "farms_manager",
                table: "hatchery_name");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_note_user_created_by",
                schema: "farms_manager",
                table: "hatchery_note");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_note_user_deleted_by",
                schema: "farms_manager",
                table: "hatchery_note");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_note_user_modified_by",
                schema: "farms_manager",
                table: "hatchery_note");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_price_user_created_by",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_price_user_deleted_by",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.DropForeignKey(
                name: "fk_hatchery_price_user_modified_by",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.DropForeignKey(
                name: "fk_henhouse_user_created_by",
                schema: "farms_manager",
                table: "henhouse");

            migrationBuilder.DropForeignKey(
                name: "fk_henhouse_user_deleted_by",
                schema: "farms_manager",
                table: "henhouse");

            migrationBuilder.DropForeignKey(
                name: "fk_henhouse_user_modified_by",
                schema: "farms_manager",
                table: "henhouse");

            migrationBuilder.DropForeignKey(
                name: "fk_insertion_user_created_by",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropForeignKey(
                name: "fk_insertion_user_deleted_by",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropForeignKey(
                name: "fk_insertion_user_modified_by",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_failure_user_created_by",
                schema: "farms_manager",
                table: "production_data_failure");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_failure_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_failure");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_failure_user_modified_by",
                schema: "farms_manager",
                table: "production_data_failure");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_flock_loss_measure_user_created_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_flock_loss_measure_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_flock_loss_measure_user_modified_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_remaining_feed_user_created_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_remaining_feed_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_remaining_feed_user_modified_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_transfer_feed_user_created_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_transfer_feed_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_transfer_feed_user_modified_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_weighing_user_created_by",
                schema: "farms_manager",
                table: "production_data_weighing");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_weighing_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_weighing");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_weighing_user_modified_by",
                schema: "farms_manager",
                table: "production_data_weighing");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_weight_standard_user_created_by",
                schema: "farms_manager",
                table: "production_data_weight_standard");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_weight_standard_user_deleted_by",
                schema: "farms_manager",
                table: "production_data_weight_standard");

            migrationBuilder.DropForeignKey(
                name: "fk_production_data_weight_standard_user_modified_by",
                schema: "farms_manager",
                table: "production_data_weight_standard");

            migrationBuilder.DropForeignKey(
                name: "fk_sale_user_created_by",
                schema: "farms_manager",
                table: "sale");

            migrationBuilder.DropForeignKey(
                name: "fk_sale_user_deleted_by",
                schema: "farms_manager",
                table: "sale");

            migrationBuilder.DropForeignKey(
                name: "fk_sale_user_modified_by",
                schema: "farms_manager",
                table: "sale");

            migrationBuilder.DropForeignKey(
                name: "fk_sale_field_extra_user_created_by",
                schema: "farms_manager",
                table: "sale_field_extra");

            migrationBuilder.DropForeignKey(
                name: "fk_sale_field_extra_user_deleted_by",
                schema: "farms_manager",
                table: "sale_field_extra");

            migrationBuilder.DropForeignKey(
                name: "fk_sale_field_extra_user_modified_by",
                schema: "farms_manager",
                table: "sale_field_extra");

            migrationBuilder.DropForeignKey(
                name: "fk_sale_invoice_user_created_by",
                schema: "farms_manager",
                table: "sale_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_sale_invoice_user_deleted_by",
                schema: "farms_manager",
                table: "sale_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_sale_invoice_user_modified_by",
                schema: "farms_manager",
                table: "sale_invoice");

            migrationBuilder.DropForeignKey(
                name: "fk_slaughterhouse_user_created_by",
                schema: "farms_manager",
                table: "slaughterhouse");

            migrationBuilder.DropForeignKey(
                name: "fk_slaughterhouse_user_deleted_by",
                schema: "farms_manager",
                table: "slaughterhouse");

            migrationBuilder.DropForeignKey(
                name: "fk_slaughterhouse_user_modified_by",
                schema: "farms_manager",
                table: "slaughterhouse");

            migrationBuilder.DropForeignKey(
                name: "fk_user_farm_user_created_by",
                schema: "farms_manager",
                table: "user_farm");

            migrationBuilder.DropForeignKey(
                name: "fk_user_farm_user_deleted_by",
                schema: "farms_manager",
                table: "user_farm");

            migrationBuilder.DropForeignKey(
                name: "fk_user_farm_user_modified_by",
                schema: "farms_manager",
                table: "user_farm");

            migrationBuilder.DropForeignKey(
                name: "fk_user_permission_user_created_by",
                schema: "farms_manager",
                table: "user_permission");

            migrationBuilder.DropForeignKey(
                name: "fk_user_permission_user_deleted_by",
                schema: "farms_manager",
                table: "user_permission");

            migrationBuilder.DropForeignKey(
                name: "fk_user_permission_user_modified_by",
                schema: "farms_manager",
                table: "user_permission");

            migrationBuilder.DropForeignKey(
                name: "fk_user_session_user_created_by",
                schema: "farms_manager",
                table: "user_session");

            migrationBuilder.DropForeignKey(
                name: "fk_user_session_user_deleted_by",
                schema: "farms_manager",
                table: "user_session");

            migrationBuilder.DropForeignKey(
                name: "fk_user_session_user_modified_by",
                schema: "farms_manager",
                table: "user_session");

            migrationBuilder.DropForeignKey(
                name: "fk_utilization_plant_user_created_by",
                schema: "farms_manager",
                table: "utilization_plant");

            migrationBuilder.DropForeignKey(
                name: "fk_utilization_plant_user_deleted_by",
                schema: "farms_manager",
                table: "utilization_plant");

            migrationBuilder.DropForeignKey(
                name: "fk_utilization_plant_user_modified_by",
                schema: "farms_manager",
                table: "utilization_plant");

            migrationBuilder.DropIndex(
                name: "ix_utilization_plant_created_by",
                schema: "farms_manager",
                table: "utilization_plant");

            migrationBuilder.DropIndex(
                name: "ix_utilization_plant_deleted_by",
                schema: "farms_manager",
                table: "utilization_plant");

            migrationBuilder.DropIndex(
                name: "ix_utilization_plant_modified_by",
                schema: "farms_manager",
                table: "utilization_plant");

            migrationBuilder.DropIndex(
                name: "ix_user_session_created_by",
                schema: "farms_manager",
                table: "user_session");

            migrationBuilder.DropIndex(
                name: "ix_user_session_deleted_by",
                schema: "farms_manager",
                table: "user_session");

            migrationBuilder.DropIndex(
                name: "ix_user_session_modified_by",
                schema: "farms_manager",
                table: "user_session");

            migrationBuilder.DropIndex(
                name: "ix_user_permission_created_by",
                schema: "farms_manager",
                table: "user_permission");

            migrationBuilder.DropIndex(
                name: "ix_user_permission_deleted_by",
                schema: "farms_manager",
                table: "user_permission");

            migrationBuilder.DropIndex(
                name: "ix_user_permission_modified_by",
                schema: "farms_manager",
                table: "user_permission");

            migrationBuilder.DropIndex(
                name: "ix_user_farm_created_by",
                schema: "farms_manager",
                table: "user_farm");

            migrationBuilder.DropIndex(
                name: "ix_user_farm_deleted_by",
                schema: "farms_manager",
                table: "user_farm");

            migrationBuilder.DropIndex(
                name: "ix_user_farm_modified_by",
                schema: "farms_manager",
                table: "user_farm");

            migrationBuilder.DropIndex(
                name: "ix_slaughterhouse_created_by",
                schema: "farms_manager",
                table: "slaughterhouse");

            migrationBuilder.DropIndex(
                name: "ix_slaughterhouse_deleted_by",
                schema: "farms_manager",
                table: "slaughterhouse");

            migrationBuilder.DropIndex(
                name: "ix_slaughterhouse_modified_by",
                schema: "farms_manager",
                table: "slaughterhouse");

            migrationBuilder.DropIndex(
                name: "ix_sale_invoice_created_by",
                schema: "farms_manager",
                table: "sale_invoice");

            migrationBuilder.DropIndex(
                name: "ix_sale_invoice_deleted_by",
                schema: "farms_manager",
                table: "sale_invoice");

            migrationBuilder.DropIndex(
                name: "ix_sale_invoice_modified_by",
                schema: "farms_manager",
                table: "sale_invoice");

            migrationBuilder.DropIndex(
                name: "ix_sale_field_extra_created_by",
                schema: "farms_manager",
                table: "sale_field_extra");

            migrationBuilder.DropIndex(
                name: "ix_sale_field_extra_deleted_by",
                schema: "farms_manager",
                table: "sale_field_extra");

            migrationBuilder.DropIndex(
                name: "ix_sale_field_extra_modified_by",
                schema: "farms_manager",
                table: "sale_field_extra");

            migrationBuilder.DropIndex(
                name: "ix_sale_created_by",
                schema: "farms_manager",
                table: "sale");

            migrationBuilder.DropIndex(
                name: "ix_sale_deleted_by",
                schema: "farms_manager",
                table: "sale");

            migrationBuilder.DropIndex(
                name: "ix_sale_modified_by",
                schema: "farms_manager",
                table: "sale");

            migrationBuilder.DropIndex(
                name: "ix_production_data_weight_standard_created_by",
                schema: "farms_manager",
                table: "production_data_weight_standard");

            migrationBuilder.DropIndex(
                name: "ix_production_data_weight_standard_deleted_by",
                schema: "farms_manager",
                table: "production_data_weight_standard");

            migrationBuilder.DropIndex(
                name: "ix_production_data_weight_standard_modified_by",
                schema: "farms_manager",
                table: "production_data_weight_standard");

            migrationBuilder.DropIndex(
                name: "ix_production_data_weighing_created_by",
                schema: "farms_manager",
                table: "production_data_weighing");

            migrationBuilder.DropIndex(
                name: "ix_production_data_weighing_deleted_by",
                schema: "farms_manager",
                table: "production_data_weighing");

            migrationBuilder.DropIndex(
                name: "ix_production_data_weighing_modified_by",
                schema: "farms_manager",
                table: "production_data_weighing");

            migrationBuilder.DropIndex(
                name: "ix_production_data_transfer_feed_created_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed");

            migrationBuilder.DropIndex(
                name: "ix_production_data_transfer_feed_deleted_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed");

            migrationBuilder.DropIndex(
                name: "ix_production_data_transfer_feed_modified_by",
                schema: "farms_manager",
                table: "production_data_transfer_feed");

            migrationBuilder.DropIndex(
                name: "ix_production_data_remaining_feed_created_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed");

            migrationBuilder.DropIndex(
                name: "ix_production_data_remaining_feed_deleted_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed");

            migrationBuilder.DropIndex(
                name: "ix_production_data_remaining_feed_modified_by",
                schema: "farms_manager",
                table: "production_data_remaining_feed");

            migrationBuilder.DropIndex(
                name: "ix_production_data_flock_loss_measure_created_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure");

            migrationBuilder.DropIndex(
                name: "ix_production_data_flock_loss_measure_deleted_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure");

            migrationBuilder.DropIndex(
                name: "ix_production_data_flock_loss_measure_modified_by",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure");

            migrationBuilder.DropIndex(
                name: "ix_production_data_failure_created_by",
                schema: "farms_manager",
                table: "production_data_failure");

            migrationBuilder.DropIndex(
                name: "ix_production_data_failure_deleted_by",
                schema: "farms_manager",
                table: "production_data_failure");

            migrationBuilder.DropIndex(
                name: "ix_production_data_failure_modified_by",
                schema: "farms_manager",
                table: "production_data_failure");

            migrationBuilder.DropIndex(
                name: "ix_insertion_created_by",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropIndex(
                name: "ix_insertion_deleted_by",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropIndex(
                name: "ix_insertion_modified_by",
                schema: "farms_manager",
                table: "insertion");

            migrationBuilder.DropIndex(
                name: "ix_henhouse_created_by",
                schema: "farms_manager",
                table: "henhouse");

            migrationBuilder.DropIndex(
                name: "ix_henhouse_deleted_by",
                schema: "farms_manager",
                table: "henhouse");

            migrationBuilder.DropIndex(
                name: "ix_henhouse_modified_by",
                schema: "farms_manager",
                table: "henhouse");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_price_created_by",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_price_deleted_by",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_price_modified_by",
                schema: "farms_manager",
                table: "hatchery_price");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_note_created_by",
                schema: "farms_manager",
                table: "hatchery_note");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_note_deleted_by",
                schema: "farms_manager",
                table: "hatchery_note");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_note_modified_by",
                schema: "farms_manager",
                table: "hatchery_note");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_name_created_by",
                schema: "farms_manager",
                table: "hatchery_name");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_name_deleted_by",
                schema: "farms_manager",
                table: "hatchery_name");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_name_modified_by",
                schema: "farms_manager",
                table: "hatchery_name");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_created_by",
                schema: "farms_manager",
                table: "hatchery");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_deleted_by",
                schema: "farms_manager",
                table: "hatchery");

            migrationBuilder.DropIndex(
                name: "ix_hatchery_modified_by",
                schema: "farms_manager",
                table: "hatchery");

            migrationBuilder.DropIndex(
                name: "ix_gas_delivery_created_by",
                schema: "farms_manager",
                table: "gas_delivery");

            migrationBuilder.DropIndex(
                name: "ix_gas_delivery_deleted_by",
                schema: "farms_manager",
                table: "gas_delivery");

            migrationBuilder.DropIndex(
                name: "ix_gas_delivery_modified_by",
                schema: "farms_manager",
                table: "gas_delivery");

            migrationBuilder.DropIndex(
                name: "ix_gas_contractor_created_by",
                schema: "farms_manager",
                table: "gas_contractor");

            migrationBuilder.DropIndex(
                name: "ix_gas_contractor_deleted_by",
                schema: "farms_manager",
                table: "gas_contractor");

            migrationBuilder.DropIndex(
                name: "ix_gas_contractor_modified_by",
                schema: "farms_manager",
                table: "gas_contractor");

            migrationBuilder.DropIndex(
                name: "ix_gas_consumption_source_created_by",
                schema: "farms_manager",
                table: "gas_consumption_source");

            migrationBuilder.DropIndex(
                name: "ix_gas_consumption_source_deleted_by",
                schema: "farms_manager",
                table: "gas_consumption_source");

            migrationBuilder.DropIndex(
                name: "ix_gas_consumption_source_modified_by",
                schema: "farms_manager",
                table: "gas_consumption_source");

            migrationBuilder.DropIndex(
                name: "ix_gas_consumption_created_by",
                schema: "farms_manager",
                table: "gas_consumption");

            migrationBuilder.DropIndex(
                name: "ix_gas_consumption_deleted_by",
                schema: "farms_manager",
                table: "gas_consumption");

            migrationBuilder.DropIndex(
                name: "ix_gas_consumption_modified_by",
                schema: "farms_manager",
                table: "gas_consumption");

            migrationBuilder.DropIndex(
                name: "ix_feed_price_created_by",
                schema: "farms_manager",
                table: "feed_price");

            migrationBuilder.DropIndex(
                name: "ix_feed_price_deleted_by",
                schema: "farms_manager",
                table: "feed_price");

            migrationBuilder.DropIndex(
                name: "ix_feed_price_modified_by",
                schema: "farms_manager",
                table: "feed_price");

            migrationBuilder.DropIndex(
                name: "ix_feed_payment_created_by",
                schema: "farms_manager",
                table: "feed_payment");

            migrationBuilder.DropIndex(
                name: "ix_feed_payment_deleted_by",
                schema: "farms_manager",
                table: "feed_payment");

            migrationBuilder.DropIndex(
                name: "ix_feed_payment_modified_by",
                schema: "farms_manager",
                table: "feed_payment");

            migrationBuilder.DropIndex(
                name: "ix_feed_name_created_by",
                schema: "farms_manager",
                table: "feed_name");

            migrationBuilder.DropIndex(
                name: "ix_feed_name_deleted_by",
                schema: "farms_manager",
                table: "feed_name");

            migrationBuilder.DropIndex(
                name: "ix_feed_name_modified_by",
                schema: "farms_manager",
                table: "feed_name");

            migrationBuilder.DropIndex(
                name: "ix_feed_invoice_correction_created_by",
                schema: "farms_manager",
                table: "feed_invoice_correction");

            migrationBuilder.DropIndex(
                name: "ix_feed_invoice_correction_deleted_by",
                schema: "farms_manager",
                table: "feed_invoice_correction");

            migrationBuilder.DropIndex(
                name: "ix_feed_invoice_correction_modified_by",
                schema: "farms_manager",
                table: "feed_invoice_correction");

            migrationBuilder.DropIndex(
                name: "ix_feed_invoice_created_by",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropIndex(
                name: "ix_feed_invoice_deleted_by",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropIndex(
                name: "ix_feed_invoice_modified_by",
                schema: "farms_manager",
                table: "feed_invoice");

            migrationBuilder.DropIndex(
                name: "ix_feed_contractor_created_by",
                schema: "farms_manager",
                table: "feed_contractor");

            migrationBuilder.DropIndex(
                name: "ix_feed_contractor_deleted_by",
                schema: "farms_manager",
                table: "feed_contractor");

            migrationBuilder.DropIndex(
                name: "ix_feed_contractor_modified_by",
                schema: "farms_manager",
                table: "feed_contractor");

            migrationBuilder.DropIndex(
                name: "ix_farm_created_by",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropIndex(
                name: "ix_farm_deleted_by",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropIndex(
                name: "ix_farm_modified_by",
                schema: "farms_manager",
                table: "farm");

            migrationBuilder.DropIndex(
                name: "ix_fallen_stock_pickup_created_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup");

            migrationBuilder.DropIndex(
                name: "ix_fallen_stock_pickup_deleted_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup");

            migrationBuilder.DropIndex(
                name: "ix_fallen_stock_pickup_modified_by",
                schema: "farms_manager",
                table: "fallen_stock_pickup");

            migrationBuilder.DropIndex(
                name: "ix_fallen_stock_created_by",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.DropIndex(
                name: "ix_fallen_stock_deleted_by",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.DropIndex(
                name: "ix_fallen_stock_modified_by",
                schema: "farms_manager",
                table: "fallen_stock");

            migrationBuilder.DropIndex(
                name: "ix_expense_type_created_by",
                schema: "farms_manager",
                table: "expense_type");

            migrationBuilder.DropIndex(
                name: "ix_expense_type_deleted_by",
                schema: "farms_manager",
                table: "expense_type");

            migrationBuilder.DropIndex(
                name: "ix_expense_type_modified_by",
                schema: "farms_manager",
                table: "expense_type");

            migrationBuilder.DropIndex(
                name: "ix_expense_production_created_by",
                schema: "farms_manager",
                table: "expense_production");

            migrationBuilder.DropIndex(
                name: "ix_expense_production_deleted_by",
                schema: "farms_manager",
                table: "expense_production");

            migrationBuilder.DropIndex(
                name: "ix_expense_production_modified_by",
                schema: "farms_manager",
                table: "expense_production");

            migrationBuilder.DropIndex(
                name: "ix_expense_contractor_created_by",
                schema: "farms_manager",
                table: "expense_contractor");

            migrationBuilder.DropIndex(
                name: "ix_expense_contractor_deleted_by",
                schema: "farms_manager",
                table: "expense_contractor");

            migrationBuilder.DropIndex(
                name: "ix_expense_contractor_modified_by",
                schema: "farms_manager",
                table: "expense_contractor");

            migrationBuilder.DropIndex(
                name: "ix_expense_advance_permission_created_by",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.DropIndex(
                name: "ix_expense_advance_permission_deleted_by",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.DropIndex(
                name: "ix_expense_advance_permission_modified_by",
                schema: "farms_manager",
                table: "expense_advance_permission");

            migrationBuilder.DropIndex(
                name: "ix_expense_advance_category_created_by",
                schema: "farms_manager",
                table: "expense_advance_category");

            migrationBuilder.DropIndex(
                name: "ix_expense_advance_category_deleted_by",
                schema: "farms_manager",
                table: "expense_advance_category");

            migrationBuilder.DropIndex(
                name: "ix_expense_advance_category_modified_by",
                schema: "farms_manager",
                table: "expense_advance_category");

            migrationBuilder.DropIndex(
                name: "ix_expense_advance_created_by",
                schema: "farms_manager",
                table: "expense_advance");

            migrationBuilder.DropIndex(
                name: "ix_expense_advance_deleted_by",
                schema: "farms_manager",
                table: "expense_advance");

            migrationBuilder.DropIndex(
                name: "ix_expense_advance_modified_by",
                schema: "farms_manager",
                table: "expense_advance");

            migrationBuilder.DropIndex(
                name: "ix_employee_reminder_created_by",
                schema: "farms_manager",
                table: "employee_reminder");

            migrationBuilder.DropIndex(
                name: "ix_employee_reminder_deleted_by",
                schema: "farms_manager",
                table: "employee_reminder");

            migrationBuilder.DropIndex(
                name: "ix_employee_reminder_modified_by",
                schema: "farms_manager",
                table: "employee_reminder");

            migrationBuilder.DropIndex(
                name: "ix_employee_payslip_created_by",
                schema: "farms_manager",
                table: "employee_payslip");

            migrationBuilder.DropIndex(
                name: "ix_employee_payslip_deleted_by",
                schema: "farms_manager",
                table: "employee_payslip");

            migrationBuilder.DropIndex(
                name: "ix_employee_payslip_modified_by",
                schema: "farms_manager",
                table: "employee_payslip");

            migrationBuilder.DropIndex(
                name: "ix_employee_file_created_by",
                schema: "farms_manager",
                table: "employee_file");

            migrationBuilder.DropIndex(
                name: "ix_employee_file_deleted_by",
                schema: "farms_manager",
                table: "employee_file");

            migrationBuilder.DropIndex(
                name: "ix_employee_file_modified_by",
                schema: "farms_manager",
                table: "employee_file");

            migrationBuilder.DropIndex(
                name: "ix_employee_created_by",
                schema: "farms_manager",
                table: "employee");

            migrationBuilder.DropIndex(
                name: "ix_employee_deleted_by",
                schema: "farms_manager",
                table: "employee");

            migrationBuilder.DropIndex(
                name: "ix_employee_modified_by",
                schema: "farms_manager",
                table: "employee");

            migrationBuilder.DropIndex(
                name: "ix_cycle_created_by",
                schema: "farms_manager",
                table: "cycle");

            migrationBuilder.DropIndex(
                name: "ix_cycle_deleted_by",
                schema: "farms_manager",
                table: "cycle");

            migrationBuilder.DropIndex(
                name: "ix_cycle_modified_by",
                schema: "farms_manager",
                table: "cycle");

            migrationBuilder.DropIndex(
                name: "ix_column_view_created_by",
                schema: "farms_manager",
                table: "column_view");

            migrationBuilder.DropIndex(
                name: "ix_column_view_deleted_by",
                schema: "farms_manager",
                table: "column_view");

            migrationBuilder.DropIndex(
                name: "ix_column_view_modified_by",
                schema: "farms_manager",
                table: "column_view");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "farms_manager",
                table: "user_farm");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "farms_manager",
                table: "user_farm");

            migrationBuilder.DropColumn(
                name: "modified_by",
                schema: "farms_manager",
                table: "user_farm");
        }
    }
}
