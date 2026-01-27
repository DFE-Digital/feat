using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class addsourcesystemandreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceReference",
                table: "Provider",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceSystem",
                table: "Provider",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceReference",
                table: "Location",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceSystem",
                table: "Location",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceReference",
                table: "EntryInstance",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceSystem",
                table: "EntryInstance",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceReference",
                table: "Entry",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceReference",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "SourceReference",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "SourceReference",
                table: "EntryInstance");

            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "EntryInstance");

            migrationBuilder.DropColumn(
                name: "SourceReference",
                table: "Entry");
        }
    }
}
