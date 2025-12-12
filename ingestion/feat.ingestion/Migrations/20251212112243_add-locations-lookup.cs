using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class addlocationslookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_postcodelatlng",
                table: "postcodelatlng");

            migrationBuilder.RenameTable(
                name: "postcodelatlng",
                newName: "PostcodeLatLong");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostcodeLatLong",
                table: "PostcodeLatLong",
                column: "Postcode");

            migrationBuilder.CreateTable(
                name: "LocationLatLong",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationLatLong", x => x.Name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationLatLong");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostcodeLatLong",
                table: "PostcodeLatLong");

            migrationBuilder.RenameTable(
                name: "PostcodeLatLong",
                newName: "postcodelatlng");

            migrationBuilder.AddPrimaryKey(
                name: "PK_postcodelatlng",
                table: "postcodelatlng",
                column: "Postcode");
        }
    }
}
