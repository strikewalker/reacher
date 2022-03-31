using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reacher.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reachable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReacherEmailAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ToEmailAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    StrikeUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CostUsdToReach = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReplyIsFree = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reachable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Email",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(998)", maxLength: 998, nullable: false),
                    FromEmailAddress = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FromEmailName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToEmailAddress = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToEmailName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OriginalEmailId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReachableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceStatus = table.Column<int>(type: "int", nullable: true),
                    CostUsd = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StrikeInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Email_Email_OriginalEmailId",
                        column: x => x.OriginalEmailId,
                        principalTable: "Email",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Email_EmailTypes_Type",
                        column: x => x.Type,
                        principalTable: "EmailTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Email_InvoiceStatuses_InvoiceStatus",
                        column: x => x.InvoiceStatus,
                        principalTable: "InvoiceStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Email_Reachable_ReachableId",
                        column: x => x.ReachableId,
                        principalTable: "Reachable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EmailTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "New" },
                    { 1, "Failed" },
                    { 2, "InboundReach" },
                    { 3, "InboundForward" },
                    { 4, "PaymentRequest" },
                    { 5, "OutboundReply" },
                    { 6, "OutboundForward" },
                    { 7, "TooSoon" }
                });

            migrationBuilder.InsertData(
                table: "InvoiceStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Requested" },
                    { 1, "Paid" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Email_FromEmailAddress",
                table: "Email",
                column: "FromEmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Email_InvoiceStatus",
                table: "Email",
                column: "InvoiceStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Email_OriginalEmailId",
                table: "Email",
                column: "OriginalEmailId");

            migrationBuilder.CreateIndex(
                name: "IX_Email_ReachableId",
                table: "Email",
                column: "ReachableId");

            migrationBuilder.CreateIndex(
                name: "IX_Email_SentDate",
                table: "Email",
                column: "SentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Email_StrikeInvoiceId",
                table: "Email",
                column: "StrikeInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Email_ToEmailAddress",
                table: "Email",
                column: "ToEmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Email_Type",
                table: "Email",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Reachable_ReacherEmailAddress",
                table: "Reachable",
                column: "ReacherEmailAddress",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Email");

            migrationBuilder.DropTable(
                name: "EmailTypes");

            migrationBuilder.DropTable(
                name: "InvoiceStatuses");

            migrationBuilder.DropTable(
                name: "Reachable");
        }
    }
}
