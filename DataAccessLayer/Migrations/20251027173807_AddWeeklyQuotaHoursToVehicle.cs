using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyQuotaHoursToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "weekly_quota_hours",
                table: "co_ownership_group");

            migrationBuilder.AddColumn<decimal>(
                name: "weekly_quota_hours",
                table: "vehicle",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "hours_used",
                table: "usage_quota",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<decimal>(
                name: "hours_limit",
                table: "usage_quota",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "weekly_quota_hours",
                table: "vehicle");

            migrationBuilder.AlterColumn<double>(
                name: "hours_used",
                table: "usage_quota",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<double>(
                name: "hours_limit",
                table: "usage_quota",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<double>(
                name: "weekly_quota_hours",
                table: "co_ownership_group",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
