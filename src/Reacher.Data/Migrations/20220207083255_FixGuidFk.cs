using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reacher.Data.Migrations
{
    public partial class FixGuidFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Email_Reachable_ReachableId",
                table: "Email");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReachableId",
                table: "Email",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Email_Reachable_ReachableId",
                table: "Email",
                column: "ReachableId",
                principalTable: "Reachable",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Email_Reachable_ReachableId",
                table: "Email");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReachableId",
                table: "Email",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Email_Reachable_ReachableId",
                table: "Email",
                column: "ReachableId",
                principalTable: "Reachable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
