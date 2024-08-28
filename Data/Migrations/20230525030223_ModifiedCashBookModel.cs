using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedCashBookModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingBalance",
                table: "CashBook");

            migrationBuilder.RenameColumn(
                name: "OpeningBalance",
                table: "CashBook",
                newName: "Amount");

            migrationBuilder.AddColumn<string>(
                name: "Particulars",
                table: "CashBook",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                table: "CashBook",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Particulars",
                table: "CashBook");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "CashBook");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "CashBook",
                newName: "OpeningBalance");

            migrationBuilder.AddColumn<decimal>(
                name: "ClosingBalance",
                table: "CashBook",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}