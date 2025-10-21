using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class AddStagingApprenticeshipCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StagingApprenticeshipCourse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 350, nullable: true),
                    NumberOfPositions = table.Column<long>(type: "bigint", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WageAmount = table.Column<double>(type: "float", nullable: true),
                    WageUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    WageAdditionalInformation = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    WorkingWeekDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 250, nullable: true),
                    HoursPerWeek = table.Column<double>(type: "float", nullable: false),
                    ExpectedDuration = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AddressLine3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AddressLine4 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AddressPostcode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AddressLatitude = table.Column<double>(type: "float", nullable: true),
                    AddressLongitude = table.Column<double>(type: "float", nullable: true),
                    ApplicationUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Distance = table.Column<double>(type: "float", nullable: true),
                    EmployerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployerWebsiteUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    EmployerContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployerContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EmployerContactEmail = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    CourseLarsCode = table.Column<int>(type: "int", nullable: false),
                    CourseTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CourseLevel = table.Column<int>(type: "int", nullable: false),
                    CourseRoute = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CourseType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprenticeshipLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ukprn = table.Column<int>(type: "int", nullable: false),
                    IsDisabilityConfident = table.Column<bool>(type: "bit", nullable: false),
                    VacancyUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    VacancyReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsNationalVacancy = table.Column<bool>(type: "bit", nullable: false),
                    IsNationalVacancyDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagingApprenticeshipCourse", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StagingApprenticeshipCourse");
        }
    }
}
