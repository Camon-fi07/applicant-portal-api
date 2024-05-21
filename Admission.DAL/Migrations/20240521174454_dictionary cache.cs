using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Admission.DAL.Migrations
{
    /// <inheritdoc />
    public partial class dictionarycache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgramPriorities_Applications_ApplicationEntityId",
                table: "ProgramPriorities");

            migrationBuilder.DropIndex(
                name: "IX_ProgramPriorities_ApplicationEntityId",
                table: "ProgramPriorities");

            migrationBuilder.DropColumn(
                name: "ApplicationEntityId",
                table: "ProgramPriorities");

            migrationBuilder.RenameColumn(
                name: "FacultyId",
                table: "ProgramPriorities",
                newName: "ApplicationId");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantEducationDocumentTypeId",
                table: "Applications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EducationLevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EducationDocumentTypeCacheEntityId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationLevels_EducationDocumentTypes_EducationDocumentTyp~",
                        column: x => x.EducationDocumentTypeCacheEntityId,
                        principalTable: "EducationDocumentTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    EducationLevelId = table.Column<int>(type: "integer", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_EducationLevels_EducationLevelId",
                        column: x => x.EducationLevelId,
                        principalTable: "EducationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Programs_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramPriorities_ApplicationId",
                table: "ProgramPriorities",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramPriorities_ProgramId",
                table: "ProgramPriorities",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_FacultyId",
                table: "Managers",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantEducationDocumentTypeId",
                table: "Applications",
                column: "ApplicantEducationDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentTypes_EducationLevelId",
                table: "EducationDocumentTypes",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_EducationDocumentTypeCacheEntityId",
                table: "EducationLevels",
                column: "EducationDocumentTypeCacheEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_EducationLevelId",
                table: "Programs",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_FacultyId",
                table: "Programs",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_EducationDocumentTypes_ApplicantEducationDocum~",
                table: "Applications",
                column: "ApplicantEducationDocumentTypeId",
                principalTable: "EducationDocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_Faculties_FacultyId",
                table: "Managers",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramPriorities_Applications_ApplicationId",
                table: "ProgramPriorities",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramPriorities_Programs_ProgramId",
                table: "ProgramPriorities",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevels_EducationLevelId",
                table: "EducationDocumentTypes",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_EducationDocumentTypes_ApplicantEducationDocum~",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Faculties_FacultyId",
                table: "Managers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramPriorities_Applications_ApplicationId",
                table: "ProgramPriorities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramPriorities_Programs_ProgramId",
                table: "ProgramPriorities");

            migrationBuilder.DropForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevels_EducationLevelId",
                table: "EducationDocumentTypes");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "Faculties");

            migrationBuilder.DropTable(
                name: "EducationLevels");

            migrationBuilder.DropTable(
                name: "EducationDocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProgramPriorities_ApplicationId",
                table: "ProgramPriorities");

            migrationBuilder.DropIndex(
                name: "IX_ProgramPriorities_ProgramId",
                table: "ProgramPriorities");

            migrationBuilder.DropIndex(
                name: "IX_Managers_FacultyId",
                table: "Managers");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ApplicantEducationDocumentTypeId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ApplicantEducationDocumentTypeId",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "ProgramPriorities",
                newName: "FacultyId");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationEntityId",
                table: "ProgramPriorities",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramPriorities_ApplicationEntityId",
                table: "ProgramPriorities",
                column: "ApplicationEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramPriorities_Applications_ApplicationEntityId",
                table: "ProgramPriorities",
                column: "ApplicationEntityId",
                principalTable: "Applications",
                principalColumn: "Id");
        }
    }
}
