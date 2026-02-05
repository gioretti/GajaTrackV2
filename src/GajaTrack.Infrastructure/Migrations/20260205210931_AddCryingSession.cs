using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GajaTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCryingSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryingSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BabyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExternalId = table.Column<string>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryingSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CryingSessions_ExternalId",
                table: "CryingSessions",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryingSessions");
        }
    }
}
