using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GeneralSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "SelectedLanguage",
                value: "en");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GeneralSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "SelectedLanguage",
                value: "en-US");
        }
    }
}
