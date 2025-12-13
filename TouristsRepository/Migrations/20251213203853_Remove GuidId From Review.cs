using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristsRepository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGuidIdFromReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_GuideProfiles_GuideId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_GuideId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "GuideId",
                table: "Reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuideId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_GuideId",
                table: "Reviews",
                column: "GuideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_GuideProfiles_GuideId",
                table: "Reviews",
                column: "GuideId",
                principalTable: "GuideProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
