using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class AddAllCoursesStagingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Staging_AllCourses",
                columns: table => new
                {
                    COURSE_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    COURSE_RUN_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PROVIDER_UKPRN = table.Column<int>(type: "int", nullable: false),
                    PROVIDER_NAME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LEARN_AIM_REF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COURSE_NAME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WHO_THIS_COURSE_IS_FOR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DELIVER_MODE = table.Column<int>(type: "int", nullable: true),
                    STUDY_MODE = table.Column<int>(type: "int", nullable: true),
                    ATTENDANCE_PATTERN = table.Column<int>(type: "int", nullable: true),
                    FLEXIBLE_STARTDATE = table.Column<bool>(type: "bit", nullable: true),
                    STARTDATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DURATION = table.Column<TimeSpan>(type: "time", nullable: true),
                    COST = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    COST_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NATIONAL = table.Column<bool>(type: "bit", nullable: true),
                    REGIONS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCATION_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCATION_ADDRESS1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCATION_ADDRESS2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCATION_COUNTY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCATION_EMAIL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCATION = table.Column<Point>(type: "geography", nullable: true),
                    LOCATION_POSTCODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCATION_TELEPHONE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCATION_TOWN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCATION_WEBSITE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COURSE_URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UPDATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ENTRY_REQUIREMENTS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HOW_YOU_WILL_BE_ASSESSED = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COURSE_TYPE = table.Column<int>(type: "int", nullable: true),
                    SECTOR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EDUCATION_LEVEL = table.Column<int>(type: "int", nullable: true),
                    AWARDING_BODY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staging_AllCourses", x => new { x.COURSE_ID, x.COURSE_RUN_ID });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Staging_AllCourses");
        }
    }
}
