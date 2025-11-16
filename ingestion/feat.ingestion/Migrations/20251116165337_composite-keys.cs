using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class compositekeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProviderLocation",
                table: "ProviderLocation");

            migrationBuilder.DropIndex(
                name: "IX_ProviderLocation_ProviderId",
                table: "ProviderLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntrySector",
                table: "EntrySector");

            migrationBuilder.DropIndex(
                name: "IX_EntrySector_EntryId",
                table: "EntrySector");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntryLocation",
                table: "EntryLocation");

            migrationBuilder.DropIndex(
                name: "IX_EntryLocation_EntryId",
                table: "EntryLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployerLocation",
                table: "EmployerLocation");

            migrationBuilder.DropIndex(
                name: "IX_EmployerLocation_EmployerId",
                table: "EmployerLocation");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProviderLocation");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EntrySector");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EntryLocation");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EmployerLocation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProviderLocation",
                table: "ProviderLocation",
                columns: new[] { "ProviderId", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntrySector",
                table: "EntrySector",
                columns: new[] { "EntryId", "SectorId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntryLocation",
                table: "EntryLocation",
                columns: new[] { "EntryId", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployerLocation",
                table: "EmployerLocation",
                columns: new[] { "EmployerId", "LocationId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProviderLocation",
                table: "ProviderLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntrySector",
                table: "EntrySector");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntryLocation",
                table: "EntryLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployerLocation",
                table: "EmployerLocation");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProviderLocation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "EntrySector",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "EntryLocation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "EmployerLocation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProviderLocation",
                table: "ProviderLocation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntrySector",
                table: "EntrySector",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntryLocation",
                table: "EntryLocation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployerLocation",
                table: "EmployerLocation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLocation_ProviderId",
                table: "ProviderLocation",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_EntrySector_EntryId",
                table: "EntrySector",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EntryLocation_EntryId",
                table: "EntryLocation",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployerLocation_EmployerId",
                table: "EmployerLocation",
                column: "EmployerId");
        }
    }
}
