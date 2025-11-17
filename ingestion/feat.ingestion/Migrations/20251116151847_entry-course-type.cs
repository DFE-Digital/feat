using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class entrycoursetype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SD 16-11-2025
            // This is needed as a weird workaround for a bug where I've
            // found that the CourseType field wasn't added in EF migrations,
            // but did exist in the tables
            
            migrationBuilder.Sql(@"IF COL_LENGTH ('Entry', 'CourseType') IS NULL
BEGIN
  ALTER TABLE Entry
    ADD CourseType int NULL
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseType",
                table: "Entry");
        }
    }
}
