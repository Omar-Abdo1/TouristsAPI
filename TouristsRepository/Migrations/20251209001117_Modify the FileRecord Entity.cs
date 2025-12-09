using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristsRepository.Migrations
{
    /// <inheritdoc />
    public partial class ModifytheFileRecordEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StoragePath",
                table: "FileRecords",
                newName: "OriginalName");

            migrationBuilder.RenameColumn(
                name: "SizeBytes",
                table: "FileRecords",
                newName: "Size");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "FileRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "FileRecords");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "FileRecords",
                newName: "SizeBytes");

            migrationBuilder.RenameColumn(
                name: "OriginalName",
                table: "FileRecords",
                newName: "StoragePath");
        }
    }
}
