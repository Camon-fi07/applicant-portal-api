using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Applicant.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addfilesentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Files",
                table: "Passports");

            migrationBuilder.DropColumn(
                name: "Files",
                table: "Educations");

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EducationEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    PassportEntityId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Educations_EducationEntityId",
                        column: x => x.EducationEntityId,
                        principalTable: "Educations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Files_Passports_PassportEntityId",
                        column: x => x.PassportEntityId,
                        principalTable: "Passports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_EducationEntityId",
                table: "Files",
                column: "EducationEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_PassportEntityId",
                table: "Files",
                column: "PassportEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

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
    }
}
