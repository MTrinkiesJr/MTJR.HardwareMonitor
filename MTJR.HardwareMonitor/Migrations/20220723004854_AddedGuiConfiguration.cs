using Microsoft.EntityFrameworkCore.Migrations;

namespace MTJR.HardwareMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddedGuiConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuiConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ShowCPULoad = table.Column<bool>(type: "boolean", nullable: false),
                    ShowCPUTemp = table.Column<bool>(type: "boolean", nullable: false),
                    ShowGPULoad = table.Column<bool>(type: "boolean", nullable: false),
                    ShowGPUTemp = table.Column<bool>(type: "boolean", nullable: false),
                    UseIoBroker = table.Column<bool>(type: "boolean", nullable: false),
                    IoBrokerHostname = table.Column<string>(type: "text", nullable: true),
                    IoBrokerPort = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuiConfigurations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuiConfigurations");
        }
    }
}
