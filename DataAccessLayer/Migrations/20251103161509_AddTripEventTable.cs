using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddTripEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "booking_id",
                table: "trip_event",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trip_event_booking_id",
                table: "trip_event",
                column: "booking_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trip_event_booking_booking_id",
                table: "trip_event",
                column: "booking_id",
                principalTable: "booking",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trip_event_booking_booking_id",
                table: "trip_event");

            migrationBuilder.DropIndex(
                name: "IX_trip_event_booking_id",
                table: "trip_event");

            migrationBuilder.DropColumn(
                name: "booking_id",
                table: "trip_event");
        }
    }
}
