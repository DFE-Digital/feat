using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class AddEntryLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudyTime",
                table: "Entry",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyTime",
                table: "Entry");
        }
    }
}
