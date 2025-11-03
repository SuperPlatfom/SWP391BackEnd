using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleRequestClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "purpose",
                table: "booking");

            migrationBuilder.AlterColumn<string>(
                name: "plate_number",
                table: "vehicle",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "model",
                table: "vehicle",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "make",
                table: "vehicle",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "color",
                table: "vehicle",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "registration_paper_url",
                table: "vehicle",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "vehicle_image_url",
                table: "vehicle",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "vehicle_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    plate_number = table.Column<string>(type: "text", nullable: false),
                    make = table.Column<string>(type: "text", nullable: false),
                    model = table.Column<string>(type: "text", nullable: false),
                    model_year = table.Column<int>(type: "integer", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false),
                    battery_capacity_kwh = table.Column<decimal>(type: "numeric", nullable: false),
                    range_km = table.Column<int>(type: "integer", nullable: false),
                    vehicle_image_url = table.Column<string>(type: "text", nullable: false),
                    registration_paper_url = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    rejection_reason = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_vehicle_request_account_created_by",
                        column: x => x.created_by,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vehicle_request_vehicle_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_plate_number",
                table: "vehicle",
                column: "plate_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_request_created_by",
                table: "vehicle_request",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_request_vehicle_id",
                table: "vehicle_request",
                column: "vehicle_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vehicle_request");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_plate_number",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "registration_paper_url",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "vehicle_image_url",
                table: "vehicle");

            migrationBuilder.AlterColumn<string>(
                name: "plate_number",
                table: "vehicle",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "model",
                table: "vehicle",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "make",
                table: "vehicle",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "color",
                table: "vehicle",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "purpose",
                table: "booking",
                type: "text",
                nullable: true);
        }
    }
}
