using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristsRepository.Migrations
{
    /// <inheritdoc />
    public partial class ImplementBookingSystemWithSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxGroupSize",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 15);

            migrationBuilder.AddColumn<int>(
                name: "GuideProfileId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TourId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TouristId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FailureMessage",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketCount",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TourScheduleId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TourSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourSchedule_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_GuideProfileId",
                table: "Reviews",
                column: "GuideProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TourId",
                table: "Reviews",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TouristId",
                table: "Reviews",
                column: "TouristId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TourScheduleId",
                table: "Bookings",
                column: "TourScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_TourSchedule_CreatedAt",
                table: "TourSchedule",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TourSchedule_TourId",
                table: "TourSchedule",
                column: "TourId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TourSchedule_TourScheduleId",
                table: "Bookings",
                column: "TourScheduleId",
                principalTable: "TourSchedule",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_GuideProfiles_GuideProfileId",
                table: "Reviews",
                column: "GuideProfileId",
                principalTable: "GuideProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_TouristProfiles_TouristId",
                table: "Reviews",
                column: "TouristId",
                principalTable: "TouristProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Tours_TourId",
                table: "Reviews",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TourSchedule_TourScheduleId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_GuideProfiles_GuideProfileId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_TouristProfiles_TouristId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Tours_TourId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "TourSchedule");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_GuideProfileId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_TourId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_TouristId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TourScheduleId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "MaxGroupSize",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "GuideProfileId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TourId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TouristId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FailureMessage",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TicketCount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TourScheduleId",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
