using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class addduingestionstateetag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscoverUni_IngestionState",
                table: "DiscoverUni_IngestionState");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "DiscoverUni_IngestionState");

            migrationBuilder.DropColumn(
                name: "LastDownloaded",
                table: "DiscoverUni_IngestionState");

            migrationBuilder.RenameTable(
                name: "DiscoverUni_IngestionState",
                newName: "DU_IngestionState");

            migrationBuilder.AddColumn<string>(
                name: "ETag",
                table: "DU_IngestionState",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DU_IngestionState",
                table: "DU_IngestionState",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DU_IngestionState",
                table: "DU_IngestionState");

            migrationBuilder.DropColumn(
                name: "ETag",
                table: "DU_IngestionState");

            migrationBuilder.RenameTable(
                name: "DU_IngestionState",
                newName: "DiscoverUni_IngestionState");

            migrationBuilder.AddColumn<double>(
                name: "FileSize",
                table: "DiscoverUni_IngestionState",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDownloaded",
                table: "DiscoverUni_IngestionState",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscoverUni_IngestionState",
                table: "DiscoverUni_IngestionState",
                column: "Id");
        }
    }
}
