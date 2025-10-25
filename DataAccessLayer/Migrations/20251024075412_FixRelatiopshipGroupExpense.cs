using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class FixRelatiopshipGroupExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_service_request_group_expense_GroupExpenseId",
                table: "service_request");

            migrationBuilder.DropForeignKey(
                name: "FK_service_request_group_expense_expense_id",
                table: "service_request");

            migrationBuilder.DropIndex(
                name: "IX_service_request_GroupExpenseId",
                table: "service_request");

            migrationBuilder.DropIndex(
                name: "IX_service_request_expense_id",
                table: "service_request");

            migrationBuilder.DropColumn(
                name: "GroupExpenseId",
                table: "service_request");

            migrationBuilder.DropColumn(
                name: "expense_id",
                table: "service_request");

            migrationBuilder.AddColumn<Guid>(
                name: "service_request_id",
                table: "group_expense",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_expense_service_request_id",
                table: "group_expense",
                column: "service_request_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_group_expense_service_request_service_request_id",
                table: "group_expense",
                column: "service_request_id",
                principalTable: "service_request",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_group_expense_service_request_service_request_id",
                table: "group_expense");

            migrationBuilder.DropIndex(
                name: "IX_group_expense_service_request_id",
                table: "group_expense");

            migrationBuilder.DropColumn(
                name: "service_request_id",
                table: "group_expense");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupExpenseId",
                table: "service_request",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "expense_id",
                table: "service_request",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_request_GroupExpenseId",
                table: "service_request",
                column: "GroupExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_expense_id",
                table: "service_request",
                column: "expense_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_service_request_group_expense_GroupExpenseId",
                table: "service_request",
                column: "GroupExpenseId",
                principalTable: "group_expense",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_request_group_expense_expense_id",
                table: "service_request",
                column: "expense_id",
                principalTable: "group_expense",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
