using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TouristsRepository.Migrations
{
    /// <inheritdoc />
    public partial class AddingTimeStampforSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_GuideProfiles_GuideId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_GuideId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "GuideId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Bookings",
                newName: "PriceAtBooking");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "TourSchedule",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingDate",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "TourSchedule");

            migrationBuilder.DropColumn(
                name: "BookingDate",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "PriceAtBooking",
                table: "Bookings",
                newName: "TotalPrice");

            migrationBuilder.AddColumn<int>(
                name: "GuideId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_GuideId",
                table: "Bookings",
                column: "GuideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_GuideProfiles_GuideId",
                table: "Bookings",
                column: "GuideId",
                principalTable: "GuideProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
