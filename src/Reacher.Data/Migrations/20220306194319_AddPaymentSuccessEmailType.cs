using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reacher.Data.Migrations
{
    public partial class AddPaymentSuccessEmailType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmailTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 8, "PaymentSuccess" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailTypes",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
