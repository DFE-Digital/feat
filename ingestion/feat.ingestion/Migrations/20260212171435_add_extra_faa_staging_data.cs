using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class add_extra_faa_staging_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployerDescription",
                table: "FAA_Apprenticeships",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullDescription",
                table: "FAA_Apprenticeships",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QualificationsSummary",
                table: "FAA_Apprenticeships",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployerDescription",
                table: "FAA_Apprenticeships");

            migrationBuilder.DropColumn(
                name: "FullDescription",
                table: "FAA_Apprenticeships");

            migrationBuilder.DropColumn(
                name: "QualificationsSummary",
                table: "FAA_Apprenticeships");
        }
    }
}
