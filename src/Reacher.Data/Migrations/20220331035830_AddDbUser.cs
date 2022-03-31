using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reacher.Data.Migrations
{
    public partial class AddDbUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Reachable",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    StrikeUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reachable_UserId",
                table: "Reachable",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reachable_User_UserId",
                table: "Reachable",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reachable_User_UserId",
                table: "Reachable");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Reachable_UserId",
                table: "Reachable");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reachable");
        }
    }
}
