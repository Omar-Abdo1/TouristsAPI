using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristsRepository.Migrations
{
    /// <inheritdoc />
    public partial class FixCircularDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatParticipants",
                table: "ChatParticipants");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ChatParticipants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatParticipants",
                table: "ChatParticipants",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_ChatId_UserId",
                table: "ChatParticipants",
                columns: new[] { "ChatId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatParticipants",
                table: "ChatParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ChatParticipants_ChatId_UserId",
                table: "ChatParticipants");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ChatParticipants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatParticipants",
                table: "ChatParticipants",
                columns: new[] { "ChatId", "UserId" });
        }
    }
}
