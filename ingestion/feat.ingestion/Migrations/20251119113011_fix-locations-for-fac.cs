using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class fixlocationsforfac : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntryLocation");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "EntryInstance",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntryInstance_LocationId",
                table: "EntryInstance",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntryInstance_Location_LocationId",
                table: "EntryInstance",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntryInstance_Location_LocationId",
                table: "EntryInstance");

            migrationBuilder.DropIndex(
                name: "IX_EntryInstance_LocationId",
                table: "EntryInstance");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "EntryInstance");

            migrationBuilder.CreateTable(
                name: "EntryLocation",
                columns: table => new
                {
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceSystem = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryLocation", x => new { x.EntryId, x.LocationId });
                    table.ForeignKey(
                        name: "FK_EntryLocation_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntryLocation_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntryLocation_LocationId",
                table: "EntryLocation",
                column: "LocationId");
        }
    }
}
