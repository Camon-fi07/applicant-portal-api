using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Admission.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Fixrelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_EducationDocumentTypes_ApplicantEducationDocum~",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevels_EducationLevelId",
                table: "EducationDocumentTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_EducationLevels_EducationDocumentTypes_EducationDocumentTyp~",
                table: "EducationLevels");

            migrationBuilder.DropIndex(
                name: "IX_EducationLevels_EducationDocumentTypeCacheEntityId",
                table: "EducationLevels");

            migrationBuilder.DropColumn(
                name: "EducationDocumentTypeCacheEntityId",
                table: "EducationLevels");

            migrationBuilder.AlterColumn<int>(
                name: "EducationLevelId",
                table: "EducationDocumentTypes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantEducationDocumentTypeId",
                table: "Applications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "LevelDocumentType",
                columns: table => new
                {
                    NextEducationLevelsId = table.Column<int>(type: "integer", nullable: false),
                    PreviousEducationLevelDocumentTypesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelDocumentType", x => new { x.NextEducationLevelsId, x.PreviousEducationLevelDocumentTypesId });
                    table.ForeignKey(
                        name: "FK_LevelDocumentType_EducationDocumentTypes_PreviousEducationL~",
                        column: x => x.PreviousEducationLevelDocumentTypesId,
                        principalTable: "EducationDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LevelDocumentType_EducationLevels_NextEducationLevelsId",
                        column: x => x.NextEducationLevelsId,
                        principalTable: "EducationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LevelDocumentType_PreviousEducationLevelDocumentTypesId",
                table: "LevelDocumentType",
                column: "PreviousEducationLevelDocumentTypesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_EducationDocumentTypes_ApplicantEducationDocum~",
                table: "Applications",
                column: "ApplicantEducationDocumentTypeId",
                principalTable: "EducationDocumentTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevels_EducationLevelId",
                table: "EducationDocumentTypes",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_EducationDocumentTypes_ApplicantEducationDocum~",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevels_EducationLevelId",
                table: "EducationDocumentTypes");

            migrationBuilder.DropTable(
                name: "LevelDocumentType");

            migrationBuilder.AddColumn<Guid>(
                name: "EducationDocumentTypeCacheEntityId",
                table: "EducationLevels",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EducationLevelId",
                table: "EducationDocumentTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantEducationDocumentTypeId",
                table: "Applications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_EducationDocumentTypeCacheEntityId",
                table: "EducationLevels",
                column: "EducationDocumentTypeCacheEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_EducationDocumentTypes_ApplicantEducationDocum~",
                table: "Applications",
                column: "ApplicantEducationDocumentTypeId",
                principalTable: "EducationDocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EducationDocumentTypes_EducationLevels_EducationLevelId",
                table: "EducationDocumentTypes",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EducationLevels_EducationDocumentTypes_EducationDocumentTyp~",
                table: "EducationLevels",
                column: "EducationDocumentTypeCacheEntityId",
                principalTable: "EducationDocumentTypes",
                principalColumn: "Id");
        }
    }
}
