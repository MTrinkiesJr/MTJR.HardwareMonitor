using Microsoft.EntityFrameworkCore.Migrations;

namespace MTJR.HardwareMonitor.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedGuiConfiguration_IoBrokerStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StateTypeConfiguration",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StateType = table.Column<int>(type: "integer", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    GuiConfigurationId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateTypeConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateTypeConfiguration_GuiConfigurations_GuiConfigurationId",
                        column: x => x.GuiConfigurationId,
                        principalTable: "GuiConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StateTypeConfiguration_GuiConfigurationId",
                table: "StateTypeConfiguration",
                column: "GuiConfigurationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StateTypeConfiguration");
        }
    }
}
