using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class addcountyandcleanname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostcodeNoSpace",
                table: "PostcodeLatLong",
                newName: "CleanPostcode");

            migrationBuilder.RenameIndex(
                name: "IX_PostcodeLatLong_PostcodeNoSpace_Expired",
                table: "PostcodeLatLong",
                newName: "IX_PostcodeLatLong_CleanPostcode_Expired");

            migrationBuilder.AddColumn<string>(
                name: "CleanName",
                table: "LocationLatLong",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "LocationLatLong",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationLatLong_CleanName",
                table: "LocationLatLong",
                column: "CleanName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LocationLatLong_CleanName",
                table: "LocationLatLong");

            migrationBuilder.DropColumn(
                name: "CleanName",
                table: "LocationLatLong");

            migrationBuilder.DropColumn(
                name: "County",
                table: "LocationLatLong");

            migrationBuilder.RenameColumn(
                name: "CleanPostcode",
                table: "PostcodeLatLong",
                newName: "PostcodeNoSpace");

            migrationBuilder.RenameIndex(
                name: "IX_PostcodeLatLong_CleanPostcode_Expired",
                table: "PostcodeLatLong",
                newName: "IX_PostcodeLatLong_PostcodeNoSpace_Expired");
        }
    }
}
