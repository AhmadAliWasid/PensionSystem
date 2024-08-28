using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class ModifyingCashBookTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "CashBook",
                newName: "ClosingBalance");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Month",
                table: "CashBook",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<decimal>(
                name: "OpeningBalance",
                table: "CashBook",
                type: "Decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpeningBalance",
                table: "CashBook");

            migrationBuilder.RenameColumn(
                name: "ClosingBalance",
                table: "CashBook",
                newName: "Balance");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Month",
                table: "CashBook",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");
        }
    }
}