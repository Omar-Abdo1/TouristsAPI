using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristsRepository.Migrations
{
    /// <inheritdoc />
    public partial class CreatedIndexOnCreatedAT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tours_CreatedAt",
                table: "Tours",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TourMedia_CreatedAt",
                table: "TourMedia",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TouristProfiles_CreatedAt",
                table: "TouristProfiles",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CreatedAt",
                table: "Reviews",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MessageVisibility_CreatedAt",
                table: "MessageVisibility",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedAt",
                table: "Messages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedAt",
                table: "Languages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_GuideProfiles_CreatedAt",
                table: "GuideProfiles",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FileRecords_CreatedAt",
                table: "FileRecords",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_CreatedAt",
                table: "Chats",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_CreatedAt",
                table: "ChatParticipants",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CreatedAt",
                table: "Bookings",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tours_CreatedAt",
                table: "Tours");

            migrationBuilder.DropIndex(
                name: "IX_TourMedia_CreatedAt",
                table: "TourMedia");

            migrationBuilder.DropIndex(
                name: "IX_TouristProfiles_CreatedAt",
                table: "TouristProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CreatedAt",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_MessageVisibility_CreatedAt",
                table: "MessageVisibility");

            migrationBuilder.DropIndex(
                name: "IX_Messages_CreatedAt",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_CreatedAt",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_GuideProfiles_CreatedAt",
                table: "GuideProfiles");

            migrationBuilder.DropIndex(
                name: "IX_FileRecords_CreatedAt",
                table: "FileRecords");

            migrationBuilder.DropIndex(
                name: "IX_Chats_CreatedAt",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_ChatParticipants_CreatedAt",
                table: "ChatParticipants");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CreatedAt",
                table: "Bookings");
        }
    }
}
