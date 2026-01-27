using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class AddFACCoursesandTLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Staging_AllCourses",
                table: "Staging_AllCourses");

            migrationBuilder.RenameTable(
                name: "Staging_AllCourses",
                newName: "FAC_AllCourses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FAC_AllCourses",
                table: "FAC_AllCourses",
                columns: new[] { "COURSE_ID", "COURSE_RUN_ID" });

            migrationBuilder.CreateTable(
                name: "FAC_Courses",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LearnAimRef = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    ProviderUkprn = table.Column<int>(type: "int", nullable: false),
                    CourseDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EntryRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WhatYoullLearn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HowYoullLearn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WhatYoullNeed = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HowYoullBeAssessed = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WhereNext = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CourseType = table.Column<int>(type: "int", nullable: true),
                    EducationLevel = table.Column<int>(type: "int", nullable: true),
                    AwardingBody = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAC_Courses", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "FAC_TLevels",
                columns: table => new
                {
                    TLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TLevelDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TLevelStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WhoFor = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EntryRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WhatYoullLearn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HowYoullLearn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HowYoullBeAssessed = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WhatYouCanDoNext = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAC_TLevels", x => x.TLevelId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAC_Courses");

            migrationBuilder.DropTable(
                name: "FAC_TLevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FAC_AllCourses",
                table: "FAC_AllCourses");

            migrationBuilder.RenameTable(
                name: "FAC_AllCourses",
                newName: "Staging_AllCourses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staging_AllCourses",
                table: "Staging_AllCourses",
                columns: new[] { "COURSE_ID", "COURSE_RUN_ID" });
        }
    }
}
