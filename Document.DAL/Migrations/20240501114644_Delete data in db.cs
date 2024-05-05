using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Deletedataindb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "FilesDetails");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "FilesDetails",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "FilesDetails");

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "FilesDetails",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
