using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class addpostcodewithoutspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostcodeNoSpace",
                table: "PostcodeLatLong",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PostcodeLatLong_PostcodeNoSpace_Expired",
                table: "PostcodeLatLong",
                columns: new[] { "PostcodeNoSpace", "Expired" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PostcodeLatLong_PostcodeNoSpace_Expired",
                table: "PostcodeLatLong");

            migrationBuilder.DropColumn(
                name: "PostcodeNoSpace",
                table: "PostcodeLatLong");
        }
    }
}
