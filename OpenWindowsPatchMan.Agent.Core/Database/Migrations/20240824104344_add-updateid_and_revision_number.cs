using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenWindowsPatchMan.Agent.Service.Migrations
{
    /// <inheritdoc />
    public partial class addupdateid_and_revision_number : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RevisionNumber",
                table: "WindowsUpdateInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpdateId",
                table: "WindowsUpdateInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevisionNumber",
                table: "WindowsUpdateInfo");

            migrationBuilder.DropColumn(
                name: "UpdateId",
                table: "WindowsUpdateInfo");
        }
    }
}
