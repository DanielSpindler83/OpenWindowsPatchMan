using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenWindowsPatchMan.Agent.Service.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WindowsUpdateInfo",
                columns: table => new
                {
                    WindowsUpdateInfoEntryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UpdateCheckTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    KBArticleIDs = table.Column<string>(type: "TEXT", nullable: false),
                    Categories = table.Column<string>(type: "TEXT", nullable: false),
                    DownloadSizeMB = table.Column<double>(type: "REAL", nullable: false),
                    MoreInfoUrls = table.Column<string>(type: "TEXT", nullable: false),
                    DatePublished = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeploymentAction = table.Column<string>(type: "TEXT", nullable: false),
                    IsBeta = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDownloaded = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInstalled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMandatory = table.Column<bool>(type: "INTEGER", nullable: false),
                    InstallationRebootBehavior = table.Column<string>(type: "TEXT", nullable: false),
                    IsUninstallable = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReleaseNotes = table.Column<string>(type: "TEXT", nullable: false),
                    UninstallationSteps = table.Column<string>(type: "TEXT", nullable: false),
                    UninstallationNotes = table.Column<string>(type: "TEXT", nullable: false),
                    SupersededUpdateIDs = table.Column<string>(type: "TEXT", nullable: false),
                    SecurityBulletinIDs = table.Column<string>(type: "TEXT", nullable: false),
                    UninstallationRebootBehavior = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    SupportUrl = table.Column<string>(type: "TEXT", nullable: false),
                    BundledUpdates = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WindowsUpdateInfo", x => x.WindowsUpdateInfoEntryId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WindowsUpdateInfo");
        }
    }
}
