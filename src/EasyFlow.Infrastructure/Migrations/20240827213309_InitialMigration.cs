using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EasyFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsWorkSoundEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsBreakSoundEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    WorkDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    BreakDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    LongBreakDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkSessionsBeforeLongBreak = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectedTheme = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectedColorTheme = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectedTagId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFocusDescriptionEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SoundVolume = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectedLanguage = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralSettings_Tags_SelectedTagId",
                        column: x => x.SelectedTagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionType = table.Column<int>(type: "INTEGER", nullable: false),
                    FinishedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TagId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Work" },
                    { 2, "Study" },
                    { 3, "Meditate" },
                    { 4, "Exercises" }
                });

            migrationBuilder.InsertData(
                table: "GeneralSettings",
                columns: new[] { "Id", "BreakDurationMinutes", "IsBreakSoundEnabled", "IsFocusDescriptionEnabled", "IsWorkSoundEnabled", "LongBreakDurationMinutes", "SelectedColorTheme", "SelectedLanguage", "SelectedTagId", "SelectedTheme", "SoundVolume", "WorkDurationMinutes", "WorkSessionsBeforeLongBreak" },
                values: new object[] { 1, 5, true, true, true, 10, 1, "en-US", 1, 0, 50, 25, 4 });

            migrationBuilder.CreateIndex(
                name: "IX_GeneralSettings_SelectedTagId",
                table: "GeneralSettings",
                column: "SelectedTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TagId",
                table: "Sessions",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
