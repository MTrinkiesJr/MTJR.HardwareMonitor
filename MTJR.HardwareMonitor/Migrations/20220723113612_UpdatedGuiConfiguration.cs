using Microsoft.EntityFrameworkCore.Migrations;

namespace MTJR.HardwareMonitor.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedGuiConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowHostname",
                table: "GuiConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInterval",
                table: "GuiConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPort",
                table: "GuiConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowHostname",
                table: "GuiConfigurations");

            migrationBuilder.DropColumn(
                name: "ShowInterval",
                table: "GuiConfigurations");

            migrationBuilder.DropColumn(
                name: "ShowPort",
                table: "GuiConfigurations");
        }
    }
}
