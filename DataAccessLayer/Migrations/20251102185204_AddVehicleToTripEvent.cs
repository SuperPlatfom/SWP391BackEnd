using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleToTripEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "vehicle_id",
                table: "trip_event",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_trip_event_vehicle_id",
                table: "trip_event",
                column: "vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trip_event_vehicle_vehicle_id",
                table: "trip_event",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trip_event_vehicle_vehicle_id",
                table: "trip_event");

            migrationBuilder.DropIndex(
                name: "IX_trip_event_vehicle_id",
                table: "trip_event");

            migrationBuilder.DropColumn(
                name: "vehicle_id",
                table: "trip_event");
        }
    }
}
