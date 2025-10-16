using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                table: "vehicle",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_created_by",
                table: "vehicle",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_account_created_by",
                table: "vehicle",
                column: "created_by",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_account_created_by",
                table: "vehicle");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_created_by",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "vehicle");
        }
    }
}
