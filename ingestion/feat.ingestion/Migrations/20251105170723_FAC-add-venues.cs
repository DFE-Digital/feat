using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class FACaddvenues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FAC_Provider",
                table: "FAC_Provider");

            migrationBuilder.RenameTable(
                name: "FAC_Provider",
                newName: "FAC_Providers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FAC_Providers",
                table: "FAC_Providers",
                column: "ProviderId");

            migrationBuilder.CreateTable(
                name: "FAC_Venues",
                columns: table => new
                {
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VenueStatus = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VenueName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProviderUkprn = table.Column<int>(type: "int", nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Town = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    County = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAC_Venues", x => x.VenueId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAC_Venues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FAC_Providers",
                table: "FAC_Providers");

            migrationBuilder.RenameTable(
                name: "FAC_Providers",
                newName: "FAC_Provider");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FAC_Provider",
                table: "FAC_Provider",
                column: "ProviderId");
        }
    }
}
