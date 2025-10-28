using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExpenseCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_group_expense_expense_category_category_id",
                table: "group_expense");

            migrationBuilder.DropTable(
                name: "expense_category");

            migrationBuilder.DropIndex(
                name: "IX_group_expense_category_id",
                table: "group_expense");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "group_expense");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "group_expense",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "expense_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_category", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_group_expense_category_id",
                table: "group_expense",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_group_expense_expense_category_category_id",
                table: "group_expense",
                column: "category_id",
                principalTable: "expense_category",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
