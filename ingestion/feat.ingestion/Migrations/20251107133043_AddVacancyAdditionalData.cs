using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class AddVacancyAdditionalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Vacancy",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NationalVacancy",
                table: "Vacancy",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalVacancyDetails",
                table: "Vacancy",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostedDate",
                table: "Vacancy",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Vacancy",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WageType",
                table: "Vacancy",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingWeekDescription",
                table: "Vacancy",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Vacancy");

            migrationBuilder.DropColumn(
                name: "NationalVacancy",
                table: "Vacancy");

            migrationBuilder.DropColumn(
                name: "NationalVacancyDetails",
                table: "Vacancy");

            migrationBuilder.DropColumn(
                name: "PostedDate",
                table: "Vacancy");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Vacancy");

            migrationBuilder.DropColumn(
                name: "WageType",
                table: "Vacancy");

            migrationBuilder.DropColumn(
                name: "WorkingWeekDescription",
                table: "Vacancy");
        }
    }
}
