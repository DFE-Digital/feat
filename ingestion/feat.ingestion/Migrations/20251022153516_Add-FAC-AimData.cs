using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class AddFACAimData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FAC_AimData",
                columns: table => new
                {
                    LearnAimRef = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    NotionalNVQLevel2 = table.Column<int>(type: "int", nullable: true),
                    LearnAimRefTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AwardOrgCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAC_AimData", x => x.LearnAimRef);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAC_AimData");
        }
    }
}
