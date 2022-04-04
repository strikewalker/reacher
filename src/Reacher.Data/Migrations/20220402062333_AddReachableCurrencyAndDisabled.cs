using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reacher.Data.Migrations
{
    public partial class AddReachableCurrencyAndDisabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Reachable");

            migrationBuilder.RenameColumn(
                name: "ReplyIsFree",
                table: "Reachable",
                newName: "Disabled");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Reachable",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.InsertData(
                table: "EmailTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 9, "Disabled" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Reachable");

            migrationBuilder.RenameColumn(
                name: "Disabled",
                table: "Reachable",
                newName: "ReplyIsFree");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Reachable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
