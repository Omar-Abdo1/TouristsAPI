using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristsRepository.Migrations
{
    /// <inheritdoc />
    public partial class addingindexonsentAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentAt",
                table: "Messages",
                column: "SentAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_SentAt",
                table: "Messages");
        }
    }
}
