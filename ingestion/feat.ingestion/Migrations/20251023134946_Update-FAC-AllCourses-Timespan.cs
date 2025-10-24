using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFACAllCoursesTimespan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DURATION",
                table: "FAC_AllCourses");
            
            migrationBuilder.AddColumn<long>(
                name: "DURATION",
                table: "FAC_AllCourses",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DURATION",
                table: "FAC_AllCourses");
            
            migrationBuilder.AddColumn<TimeSpan>(
                name: "DURATION",
                table: "FAC_AllCourses",
                type: "time",
                nullable: true);
        }
    }
}
