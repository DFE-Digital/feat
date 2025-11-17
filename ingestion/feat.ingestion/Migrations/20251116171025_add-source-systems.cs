using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class addsourcesystems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceSystem",
                table: "ProviderLocation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceSystem",
                table: "EntrySector",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceSystem",
                table: "EntryLocation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceSystem",
                table: "EntryCost",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "ProviderLocation");

            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "EntrySector");

            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "EntryLocation");

            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "EntryCost");
        }
    }
}
