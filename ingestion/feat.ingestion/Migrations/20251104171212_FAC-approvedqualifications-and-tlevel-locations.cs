using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class FACapprovedqualificationsandtlevellocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FAC_ApprovedQualifications",
                columns: table => new
                {
                    QualificationNumber = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: true),
                    QualificationType = table.Column<int>(type: "int", nullable: true),
                    SectorSubjectArea = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Age1416_FundingAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Age1619_FundingAvailable = table.Column<bool>(type: "bit", nullable: false),
                    LocalFlexibilities_FundingAvailable = table.Column<bool>(type: "bit", nullable: false),
                    LegalEntitlementL2L3_FundingAvailable = table.Column<bool>(type: "bit", nullable: false),
                    LegalEntitlementEnglishandMaths_FundingAvailable = table.Column<bool>(type: "bit", nullable: false),
                    DigitalEntitlement_FundingAvailable = table.Column<bool>(type: "bit", nullable: false),
                    LifelongLearningEntitlement_FundingAvailable = table.Column<bool>(type: "bit", nullable: false),
                    AdvancedLearnerLoans_FundingAvailable = table.Column<bool>(type: "bit", nullable: false),
                    FreeCoursesForJobs_FundingAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAC_ApprovedQualifications", x => x.QualificationNumber);
                });

            migrationBuilder.CreateTable(
                name: "FAC_TLevelLocations",
                columns: table => new
                {
                    TLevelLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TLevelLocationStatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAC_TLevelLocations", x => x.TLevelLocationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAC_ApprovedQualifications");

            migrationBuilder.DropTable(
                name: "FAC_TLevelLocations");
        }
    }
}
