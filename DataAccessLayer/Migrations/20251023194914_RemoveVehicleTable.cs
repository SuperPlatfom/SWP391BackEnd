using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVehicleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trip_event_usage_session_session_id",
                table: "trip_event");

            migrationBuilder.DropTable(
                name: "usage_session");

            migrationBuilder.DropIndex(
                name: "IX_trip_event_session_id",
                table: "trip_event");

            migrationBuilder.DropColumn(
                name: "telematics_device_id",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "session_id",
                table: "trip_event");

            migrationBuilder.AddColumn<Guid>(
                name: "vehicle_id",
                table: "usage_quota",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_usage_quota_vehicle_id",
                table: "usage_quota",
                column: "vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "FK_usage_quota_vehicle_vehicle_id",
                table: "usage_quota",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usage_quota_vehicle_vehicle_id",
                table: "usage_quota");

            migrationBuilder.DropIndex(
                name: "IX_usage_quota_vehicle_id",
                table: "usage_quota");

            migrationBuilder.DropColumn(
                name: "vehicle_id",
                table: "usage_quota");

            migrationBuilder.AddColumn<string>(
                name: "telematics_device_id",
                table: "vehicle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "session_id",
                table: "trip_event",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "usage_session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usage_session", x => x.id);
                    table.ForeignKey(
                        name: "FK_usage_session_account_user_id",
                        column: x => x.user_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usage_session_booking_booking_id",
                        column: x => x.booking_id,
                        principalTable: "booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usage_session_co_ownership_group_group_id",
                        column: x => x.group_id,
                        principalTable: "co_ownership_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usage_session_vehicle_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_event_session_id",
                table: "trip_event",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_usage_session_booking_id",
                table: "usage_session",
                column: "booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usage_session_group_id",
                table: "usage_session",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_usage_session_user_id",
                table: "usage_session",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_usage_session_vehicle_id",
                table: "usage_session",
                column: "vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trip_event_usage_session_session_id",
                table: "trip_event",
                column: "session_id",
                principalTable: "usage_session",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
