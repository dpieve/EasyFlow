using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DashboardSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DashboardDisplayType",
                table: "GeneralSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DashboardFilterPeriod",
                table: "GeneralSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DashboardSessionType",
                table: "GeneralSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "GeneralSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DashboardDisplayType", "DashboardFilterPeriod", "DashboardSessionType" },
                values: new object[] { 0, 7, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DashboardDisplayType",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "DashboardFilterPeriod",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "DashboardSessionType",
                table: "GeneralSettings");
        }
    }
}
