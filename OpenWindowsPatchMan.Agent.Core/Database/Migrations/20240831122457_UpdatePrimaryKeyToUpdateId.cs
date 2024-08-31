using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenWindowsPatchMan.Agent.Service.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePrimaryKeyToUpdateId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WindowsUpdateInfo",
                table: "WindowsUpdateInfo");

            migrationBuilder.DropColumn(
                name: "WindowsUpdateInfoEntryId",
                table: "WindowsUpdateInfo");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FirstSeenTime",
                table: "WindowsUpdateInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WindowsUpdateInfo",
                table: "WindowsUpdateInfo",
                column: "UpdateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WindowsUpdateInfo",
                table: "WindowsUpdateInfo");

            migrationBuilder.DropColumn(
                name: "FirstSeenTime",
                table: "WindowsUpdateInfo");

            migrationBuilder.AddColumn<int>(
                name: "WindowsUpdateInfoEntryId",
                table: "WindowsUpdateInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WindowsUpdateInfo",
                table: "WindowsUpdateInfo",
                column: "WindowsUpdateInfoEntryId");
        }
    }
}
