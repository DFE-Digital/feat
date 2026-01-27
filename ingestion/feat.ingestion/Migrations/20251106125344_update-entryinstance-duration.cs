using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class updateentryinstanceduration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("Duration", "EntryInstance");
            
            migrationBuilder.AddColumn<long>(
                name: "Duration",
                table: "EntryInstance",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DURATION",
                table: "EntryInstance");
            
            migrationBuilder.AddColumn<TimeSpan>(
                name: "DURATION",
                table: "EntryInstance",
                type: "time",
                nullable: true);
        }
    }
}
