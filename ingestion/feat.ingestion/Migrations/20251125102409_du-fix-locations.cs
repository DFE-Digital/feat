using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class dufixlocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DU_Institutions",
                table: "DU_Institutions");

            migrationBuilder.AddColumn<int>(
                name: "PubUKPRN",
                table: "DU_Institutions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DU_Institutions",
                table: "DU_Institutions",
                columns: new[] { "UKPRN", "PubUKPRN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DU_Institutions",
                table: "DU_Institutions");

            migrationBuilder.DropColumn(
                name: "PubUKPRN",
                table: "DU_Institutions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DU_Institutions",
                table: "DU_Institutions",
                column: "UKPRN");
        }
    }
}
