using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingMonthToHBLPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Month",
                table: "HBLPayments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_HBLPayments_Month_AccountNumber",
                table: "HBLPayments",
                columns: new[] { "Month", "AccountNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HBLPayments_Month_PensionerId",
                table: "HBLPayments",
                columns: new[] { "Month", "PensionerId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HBLPayments_Month_AccountNumber",
                table: "HBLPayments");

            migrationBuilder.DropIndex(
                name: "IX_HBLPayments_Month_PensionerId",
                table: "HBLPayments");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "HBLPayments");
        }
    }
}