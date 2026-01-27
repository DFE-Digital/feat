using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class addsourcesystemtosector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Sector");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Sector");

            migrationBuilder.AddColumn<int>(
                name: "SourceSystem",
                table: "Sector",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "Sector");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Sector",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Sector",
                type: "datetime2",
                nullable: true);
        }
    }
}
