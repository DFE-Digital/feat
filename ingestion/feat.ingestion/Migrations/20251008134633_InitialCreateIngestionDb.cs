using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateIngestionDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address3 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address4 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    County = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    GeoLocation = table.Column<Point>(type: "geography", nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Town = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(2083)", maxLength: 2083, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PUBUKPRN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UKPRN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LegalEntityName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TradingName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OtherNames = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sector", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployerLocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployerLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployerLocation_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployerLocation_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SecondaryReference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlexibleStart = table.Column<bool>(type: "bit", nullable: false),
                    AttendancePattern = table.Column<int>(type: "int", nullable: true),
                    URL = table.Column<string>(type: "nvarchar(2083)", maxLength: 2083, nullable: false),
                    SourceUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntryRequirements = table.Column<string>(type: "nvarchar(2083)", maxLength: 2083, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entry_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderLocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderLocation_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderLocation_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryCost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryCost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntryCost_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryInstance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    StudyMode = table.Column<int>(type: "int", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntryInstance_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryLocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntryLocation_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntryLocation_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntrySector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntrySector", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntrySector_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntrySector_Sector_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sector",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniversityCourse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Foundation = table.Column<bool>(type: "bit", nullable: true),
                    Honours = table.Column<bool>(type: "bit", nullable: true),
                    Nhs = table.Column<bool>(type: "bit", nullable: true),
                    Sandwich = table.Column<bool>(type: "bit", nullable: true),
                    YearAbroad = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversityCourse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UniversityCourse_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vacancy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Positions = table.Column<short>(type: "smallint", nullable: true),
                    Wage = table.Column<decimal>(type: "money", nullable: true),
                    WageUnit = table.Column<int>(type: "int", nullable: true),
                    HoursPerWeek = table.Column<int>(type: "int", nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacancy_Employer_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vacancy_Entry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployerLocation_EmployerId",
                table: "EmployerLocation",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployerLocation_LocationId",
                table: "EmployerLocation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Entry_ProviderId",
                table: "Entry",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryCost_EntryId",
                table: "EntryCost",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryInstance_EntryId",
                table: "EntryInstance",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryLocation_EntryId",
                table: "EntryLocation",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryLocation_LocationId",
                table: "EntryLocation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EntrySector_EntryId",
                table: "EntrySector",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EntrySector_SectorId",
                table: "EntrySector",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLocation_LocationId",
                table: "ProviderLocation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLocation_ProviderId",
                table: "ProviderLocation",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversityCourse_EntryId",
                table: "UniversityCourse",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancy_EmployerId",
                table: "Vacancy",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancy_EntryId",
                table: "Vacancy",
                column: "EntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployerLocation");

            migrationBuilder.DropTable(
                name: "EntryCost");

            migrationBuilder.DropTable(
                name: "EntryInstance");

            migrationBuilder.DropTable(
                name: "EntryLocation");

            migrationBuilder.DropTable(
                name: "EntrySector");

            migrationBuilder.DropTable(
                name: "ProviderLocation");

            migrationBuilder.DropTable(
                name: "UniversityCourse");

            migrationBuilder.DropTable(
                name: "Vacancy");

            migrationBuilder.DropTable(
                name: "Sector");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Employer");

            migrationBuilder.DropTable(
                name: "Entry");

            migrationBuilder.DropTable(
                name: "Provider");
        }
    }
}
