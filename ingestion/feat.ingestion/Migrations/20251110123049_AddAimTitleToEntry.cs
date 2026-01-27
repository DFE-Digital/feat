using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class AddAimTitleToEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AimOrAltTitle",
                table: "Entry",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AimOrAltTitle",
                table: "Entry");
        }
    }
}
