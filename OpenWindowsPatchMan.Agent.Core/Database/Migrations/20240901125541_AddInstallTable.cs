using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenWindowsPatchMan.Agent.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddInstallTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UpdateInstallation",
                columns: table => new
                {
                    UpdateInstallationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UpdateId = table.Column<string>(type: "TEXT", nullable: false),
                    InstallationStatus = table.Column<string>(type: "TEXT", nullable: false),
                    InstallationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InstallationResultCode = table.Column<string>(type: "TEXT", nullable: false),
                    RebootRequired = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateInstallation", x => x.UpdateInstallationId);
                    table.ForeignKey(
                        name: "FK_UpdateInstallation_WindowsUpdateInfo_UpdateId",
                        column: x => x.UpdateId,
                        principalTable: "WindowsUpdateInfo",
                        principalColumn: "UpdateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UpdateInstallation_UpdateId",
                table: "UpdateInstallation",
                column: "UpdateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UpdateInstallation");
        }
    }
}
