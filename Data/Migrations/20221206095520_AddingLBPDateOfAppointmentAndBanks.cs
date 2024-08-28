using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingLBPDateOfAppointmentAndBanks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "Pensioner",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfAppointment",
                table: "Pensioner",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "LastBasicPay",
                table: "Pensioner",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Pensioner_BankId",
                table: "Pensioner",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pensioner_Banks_BankId",
                table: "Pensioner",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pensioner_Banks_BankId",
                table: "Pensioner");

            migrationBuilder.DropIndex(
                name: "IX_Pensioner_BankId",
                table: "Pensioner");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Pensioner");

            migrationBuilder.DropColumn(
                name: "DateOfAppointment",
                table: "Pensioner");

            migrationBuilder.DropColumn(
                name: "LastBasicPay",
                table: "Pensioner");
        }
    }
}