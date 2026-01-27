using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class dufixlocations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DU_CourseLocations",
                table: "DU_CourseLocations");

            migrationBuilder.AddColumn<int>(
                name: "PubUKPRN",
                table: "DU_CourseLocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DU_CourseLocations",
                table: "DU_CourseLocations",
                columns: new[] { "UKPRN", "PubUKPRN", "CourseId", "StudyMode", "LocationId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DU_CourseLocations",
                table: "DU_CourseLocations");

            migrationBuilder.DropColumn(
                name: "PubUKPRN",
                table: "DU_CourseLocations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DU_CourseLocations",
                table: "DU_CourseLocations",
                columns: new[] { "UKPRN", "CourseId", "StudyMode", "LocationId" });
        }
    }
}
