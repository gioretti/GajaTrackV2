using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GajaTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBottleAndDiaperTimeColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "DiaperChanges",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "BottleFeeds",
                newName: "Time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "DiaperChanges",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "BottleFeeds",
                newName: "StartTime");
        }
    }
}