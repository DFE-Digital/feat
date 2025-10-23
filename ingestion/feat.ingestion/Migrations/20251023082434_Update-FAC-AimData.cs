using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFACAimData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotionalNVQLevel2",
                table: "FAC_AimData",
                newName: "NotionalNVQLevelv2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotionalNVQLevelv2",
                table: "FAC_AimData",
                newName: "NotionalNVQLevel2");
        }
    }
}
