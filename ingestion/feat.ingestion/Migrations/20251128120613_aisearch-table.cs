using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class aisearchtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiSearchEntries",
                columns: table => new
                {
                    InstanceId = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Id = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LearningAimTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sector = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    EntryType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QualificationLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LearningMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CourseHours = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudyTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<Point>(type: "geography", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiSearchEntries", x => x.InstanceId)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiSearchEntries");
        }
    }
}
