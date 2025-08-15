using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustFlockLossMeasureEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "flock_loss1percentage",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure");

            migrationBuilder.DropColumn(
                name: "flock_loss2percentage",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure");

            migrationBuilder.DropColumn(
                name: "flock_loss3percentage",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure");

            migrationBuilder.DropColumn(
                name: "flock_loss4percentage",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "flock_loss1percentage",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "flock_loss2percentage",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "flock_loss3percentage",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "flock_loss4percentage",
                schema: "farms_manager",
                table: "production_data_flock_loss_measure",
                type: "numeric",
                nullable: true);
        }
    }
}
