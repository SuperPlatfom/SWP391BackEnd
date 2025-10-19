using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddContractAndSigner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "e_contract",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    effective_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    signed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reviewed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    review_note = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_e_contract", x => x.id);
                    table.ForeignKey(
                        name: "FK_e_contract_account_created_by",
                        column: x => x.created_by,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_e_contract_co_ownership_group_group_id",
                        column: x => x.group_id,
                        principalTable: "co_ownership_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_e_contract_contract_template_template_id",
                        column: x => x.template_id,
                        principalTable: "contract_template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_e_contract_vehicle_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "e_contract_signer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    otp_code = table.Column<string>(type: "text", nullable: true),
                    otp_sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    otp_verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_signed = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    signed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_e_contract_signer", x => x.id);
                    table.ForeignKey(
                        name: "FK_e_contract_signer_account_user_id",
                        column: x => x.user_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_e_contract_signer_e_contract_contract_id",
                        column: x => x.contract_id,
                        principalTable: "e_contract",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_e_contract_created_by",
                table: "e_contract",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_e_contract_group_id",
                table: "e_contract",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_e_contract_template_id",
                table: "e_contract",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_e_contract_vehicle_id",
                table: "e_contract",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_e_contract_signer_contract_id",
                table: "e_contract_signer",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "IX_e_contract_signer_user_id",
                table: "e_contract_signer",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "e_contract_signer");

            migrationBuilder.DropTable(
                name: "e_contract");
        }
    }
}
