using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class duaddaimandcourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DU_Location",
                table: "DU_Location");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DU_Institution",
                table: "DU_Institution");

            migrationBuilder.RenameTable(
                name: "DU_Location",
                newName: "DU_Locations");

            migrationBuilder.RenameTable(
                name: "DU_Institution",
                newName: "DU_Institutions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DU_Locations",
                table: "DU_Locations",
                columns: new[] { "UKPRN", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DU_Institutions",
                table: "DU_Institutions",
                column: "UKPRN");

            migrationBuilder.CreateTable(
                name: "DU_Aims",
                columns: table => new
                {
                    AimCode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DU_Aims", x => x.AimCode);
                });

            migrationBuilder.CreateTable(
                name: "DU_Courses",
                columns: table => new
                {
                    UKPRN = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudyMode = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AssessmentUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CostsUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CourseUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LearningUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DistanceLearning = table.Column<bool>(type: "bit", nullable: true),
                    FoundationYear = table.Column<bool>(type: "bit", nullable: true),
                    Honours = table.Column<bool>(type: "bit", nullable: true),
                    NHS = table.Column<bool>(type: "bit", nullable: true),
                    Sandwich = table.Column<bool>(type: "bit", nullable: true),
                    YearAbroad = table.Column<bool>(type: "bit", nullable: true),
                    NumberOfYears = table.Column<bool>(type: "bit", nullable: true),
                    Hecos = table.Column<int>(type: "int", nullable: true),
                    Hecos2 = table.Column<int>(type: "int", nullable: true),
                    Hecos3 = table.Column<int>(type: "int", nullable: true),
                    Hecos4 = table.Column<int>(type: "int", nullable: true),
                    Hecos5 = table.Column<int>(type: "int", nullable: true),
                    Aim = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DU_Courses", x => new { x.UKPRN, x.CourseId, x.StudyMode });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DU_Aims");

            migrationBuilder.DropTable(
                name: "DU_Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DU_Locations",
                table: "DU_Locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DU_Institutions",
                table: "DU_Institutions");

            migrationBuilder.RenameTable(
                name: "DU_Locations",
                newName: "DU_Location");

            migrationBuilder.RenameTable(
                name: "DU_Institutions",
                newName: "DU_Institution");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DU_Location",
                table: "DU_Location",
                columns: new[] { "UKPRN", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DU_Institution",
                table: "DU_Institution",
                column: "UKPRN");
        }
    }
}
