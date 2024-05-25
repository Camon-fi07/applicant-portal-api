using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Applicant.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addemail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Educations_EducationId",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Passports_PassportId",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Passports");

            migrationBuilder.RenameColumn(
                name: "EducationDocumentTypeId",
                table: "Educations",
                newName: "EducationDocumentId");

            migrationBuilder.AlterColumn<Guid>(
                name: "PassportId",
                table: "Applicants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EducationId",
                table: "Applicants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Applicants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EducationLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevels", x => x.Id);
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
                    table.ForeignKey(
                        name: "FK_EducationDocumentTypes_EducationLevels_EducationLevelId",
                        column: x => x.EducationLevelId,
                        principalTable: "EducationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_EducationDocumentId",
                table: "Educations",
                column: "EducationDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentTypes_EducationLevelId",
                table: "EducationDocumentTypes",
                column: "EducationLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Educations_EducationId",
                table: "Applicants",
                column: "EducationId",
                principalTable: "Educations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Passports_PassportId",
                table: "Applicants",
                column: "PassportId",
                principalTable: "Passports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_EducationDocumentTypes_EducationDocumentId",
                table: "Educations",
                column: "EducationDocumentId",
                principalTable: "EducationDocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Educations_EducationId",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Passports_PassportId",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_EducationDocumentTypes_EducationDocumentId",
                table: "Educations");

            migrationBuilder.DropTable(
                name: "EducationDocumentTypes");

            migrationBuilder.DropTable(
                name: "EducationLevels");

            migrationBuilder.DropIndex(
                name: "IX_Educations_EducationDocumentId",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Applicants");

            migrationBuilder.RenameColumn(
                name: "EducationDocumentId",
                table: "Educations",
                newName: "EducationDocumentTypeId");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Passports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "PassportId",
                table: "Applicants",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "EducationId",
                table: "Applicants",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Educations_EducationId",
                table: "Applicants",
                column: "EducationId",
                principalTable: "Educations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Passports_PassportId",
                table: "Applicants",
                column: "PassportId",
                principalTable: "Passports",
                principalColumn: "Id");
        }
    }
}
