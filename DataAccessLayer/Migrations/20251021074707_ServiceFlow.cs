using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ServiceFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "expense_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "service_center",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_center", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "group_expense",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    incurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    invoice_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_expense", x => x.id);
                    table.ForeignKey(
                        name: "FK_group_expense_co_ownership_group_group_id",
                        column: x => x.group_id,
                        principalTable: "co_ownership_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_expense_expense_category_category_id",
                        column: x => x.category_id,
                        principalTable: "expense_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "member_invoice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    expense_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    amount_paid = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_invoice", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_invoice_account_user_id",
                        column: x => x.user_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_member_invoice_co_ownership_group_group_id",
                        column: x => x.group_id,
                        principalTable: "co_ownership_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_member_invoice_group_expense_expense_id",
                        column: x => x.expense_id,
                        principalTable: "group_expense",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cost_estimate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    inspection_scheduled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    service_center_id = table.Column<Guid>(type: "uuid", nullable: true),
                    inspection_notes = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    expense_id = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GroupExpenseId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_service_request_account_created_by",
                        column: x => x.created_by,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_service_request_account_technician_id",
                        column: x => x.technician_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_service_request_co_ownership_group_group_id",
                        column: x => x.group_id,
                        principalTable: "co_ownership_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_service_request_group_expense_GroupExpenseId",
                        column: x => x.GroupExpenseId,
                        principalTable: "group_expense",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_service_request_group_expense_expense_id",
                        column: x => x.expense_id,
                        principalTable: "group_expense",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_service_request_service_center_service_center_id",
                        column: x => x.service_center_id,
                        principalTable: "service_center",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_service_request_vehicle_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    payment_method = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    transaction_code = table.Column<string>(type: "text", nullable: false),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_account_user_id",
                        column: x => x.user_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payment_member_invoice_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "member_invoice",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "service_job",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technician_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    report_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_job", x => x.id);
                    table.ForeignKey(
                        name: "FK_service_job_account_technician_id",
                        column: x => x.technician_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_service_job_service_request_request_id",
                        column: x => x.request_id,
                        principalTable: "service_request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_request_confirmation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    decision = table.Column<string>(type: "text", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true),
                    decided_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_request_confirmation", x => x.id);
                    table.ForeignKey(
                        name: "FK_service_request_confirmation_account_user_id",
                        column: x => x.user_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_service_request_confirmation_service_request_request_id",
                        column: x => x.request_id,
                        principalTable: "service_request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payos_transaction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_code = table.Column<string>(type: "text", nullable: false),
                    qr_code_url = table.Column<string>(type: "text", nullable: false),
                    deeplink_url = table.Column<string>(type: "text", nullable: false),
                    expired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    webhook_received_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payos_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_payos_transaction_payment_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_group_expense_category_id",
                table: "group_expense",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_group_expense_group_id",
                table: "group_expense",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_invoice_expense_id",
                table: "member_invoice",
                column: "expense_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_invoice_group_id",
                table: "member_invoice",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_invoice_user_id",
                table: "member_invoice",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_invoice_id",
                table: "payment",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_user_id",
                table: "payment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_payos_transaction_payment_id",
                table: "payos_transaction",
                column: "payment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_job_request_id",
                table: "service_job",
                column: "request_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_job_technician_id",
                table: "service_job",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_GroupExpenseId",
                table: "service_request",
                column: "GroupExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_created_by",
                table: "service_request",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_expense_id",
                table: "service_request",
                column: "expense_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_request_group_id",
                table: "service_request",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_service_center_id",
                table: "service_request",
                column: "service_center_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_technician_id",
                table: "service_request",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_vehicle_id",
                table: "service_request",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_confirmation_request_id",
                table: "service_request_confirmation",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_confirmation_user_id",
                table: "service_request_confirmation",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payos_transaction");

            migrationBuilder.DropTable(
                name: "service_job");

            migrationBuilder.DropTable(
                name: "service_request_confirmation");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "service_request");

            migrationBuilder.DropTable(
                name: "member_invoice");

            migrationBuilder.DropTable(
                name: "service_center");

            migrationBuilder.DropTable(
                name: "group_expense");

            migrationBuilder.DropTable(
                name: "expense_category");
        }
    }
}
