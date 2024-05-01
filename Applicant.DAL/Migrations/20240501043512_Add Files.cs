using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Applicant.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Passports");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Educations");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Files",
                table: "Passports",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Files",
                table: "Educations",
                type: "uuid[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Files",
                table: "Passports");

            migrationBuilder.DropColumn(
                name: "Files",
                table: "Educations");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "Passports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "Educations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
