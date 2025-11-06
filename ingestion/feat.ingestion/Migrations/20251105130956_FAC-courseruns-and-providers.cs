using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class FACcourserunsandproviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FAC_CourseRuns",
                columns: table => new
                {
                    CourseRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseRunStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryMode = table.Column<int>(type: "int", nullable: false),
                    FlexibleStartDate = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourseWebsite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CostDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: true),
                    StudyMode = table.Column<int>(type: "int", nullable: true),
                    AttendancePattern = table.Column<int>(type: "int", nullable: true),
                    National = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAC_CourseRuns", x => x.CourseRunId);
                });

            migrationBuilder.CreateTable(
                name: "FAC_Provider",
                columns: table => new
                {
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ukprn = table.Column<int>(type: "int", nullable: false),
                    ProviderStatus = table.Column<int>(type: "int", nullable: false),
                    ProviderType = table.Column<int>(type: "int", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CourseDirectoryName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TradingName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAC_Provider", x => x.ProviderId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAC_CourseRuns");

            migrationBuilder.DropTable(
                name: "FAC_Provider");
        }
    }
}
