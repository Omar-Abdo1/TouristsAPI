using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristsRepository.Migrations
{
    /// <inheritdoc />
    public partial class AddingIndexonMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Messages_LastMessageId1",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_LastMessageId1",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastMessageId1",
                table: "Chats");

            migrationBuilder.CreateIndex(
                name: "IX_MessageVisibility_MessageId_UserId",
                table: "MessageVisibility",
                columns: new[] { "MessageId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId_SentAt",
                table: "Messages",
                columns: new[] { "ChatId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats",
                column: "LastMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Messages_LastMessageId",
                table: "Chats",
                column: "LastMessageId",
                principalTable: "Messages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Messages_LastMessageId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_MessageVisibility_MessageId_UserId",
                table: "MessageVisibility");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId_SentAt",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats");

            migrationBuilder.AddColumn<int>(
                name: "LastMessageId1",
                table: "Chats",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastMessageId1",
                table: "Chats",
                column: "LastMessageId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Messages_LastMessageId1",
                table: "Chats",
                column: "LastMessageId1",
                principalTable: "Messages",
                principalColumn: "Id");
        }
    }
}
