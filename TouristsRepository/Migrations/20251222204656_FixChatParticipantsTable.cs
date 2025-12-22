using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristsRepository.Migrations
{
    /// <inheritdoc />
    public partial class FixChatParticipantsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.CreateTable(
        name: "ChatParticipants",
        columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"), // <--- THE KEY FIX (Auto-Increment)
            
            ChatId = table.Column<int>(type: "int", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            
            // Additional Properties
            LastSeenMessageId = table.Column<int>(type: "int", nullable: true),
            JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
            
            // Base Entity Columns (Audit)
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_ChatParticipants", x => x.Id);
            
            // Foreign Keys
            table.ForeignKey(
                name: "FK_ChatParticipants_Chats_ChatId",
                column: x => x.ChatId,
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade); // Deleting a Chat kicks out participants

            table.ForeignKey(
                name: "FK_ChatParticipants_Users_UserId",
                column: x => x.UserId,
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade); // Deleting a User removes them from participants
        });

    // 2. Add Indexes
    migrationBuilder.CreateIndex(
        name: "IX_ChatParticipants_ChatId_UserId",
        table: "ChatParticipants",
        columns: new[] { "ChatId", "UserId" },
        unique: true); // <--- Prevents duplicates in the same chat

    migrationBuilder.CreateIndex(
        name: "IX_ChatParticipants_UserId",
        table: "ChatParticipants",
        column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatParticipants_Chats_ChatId",
                table: "ChatParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatParticipants_Users_UserId",
                table: "ChatParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatParticipants",
                table: "ChatParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ChatParticipants_ChatId_UserId",
                table: "ChatParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ChatParticipants_CreatedAt",
                table: "ChatParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ChatParticipants_UserId",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "LastSeenMessageId",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ChatParticipants");

            migrationBuilder.RenameTable(
                name: "ChatParticipants",
                newName: "ChatParticipant");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ChatParticipant",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "ChatParticipant",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatParticipant_Chats_ChatId",
                table: "ChatParticipant",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatParticipant_Users_UserId",
                table: "ChatParticipant",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
