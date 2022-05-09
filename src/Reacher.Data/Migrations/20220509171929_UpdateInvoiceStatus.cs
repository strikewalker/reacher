using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reacher.Data.Migrations
{
    public partial class UpdateInvoiceStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "InvoiceStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Forwarded" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InvoiceStatuses",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
