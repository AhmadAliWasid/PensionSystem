using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class ImprovingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PensionerPayment_Pensioner_PensionerId",
                table: "PensionerPayment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PensionerPayment",
                table: "PensionerPayment");

            migrationBuilder.RenameTable(
                name: "PensionerPayment",
                newName: "PensionerPayments");

            migrationBuilder.RenameIndex(
                name: "IX_PensionerPayment_PensionerId_Month",
                table: "PensionerPayments",
                newName: "IX_PensionerPayments_PensionerId_Month");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PensionerPayments",
                table: "PensionerPayments",
                column: "PensionerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PensionerPayments_Pensioner_PensionerId",
                table: "PensionerPayments",
                column: "PensionerId",
                principalTable: "Pensioner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PensionerPayments_Pensioner_PensionerId",
                table: "PensionerPayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PensionerPayments",
                table: "PensionerPayments");

            migrationBuilder.RenameTable(
                name: "PensionerPayments",
                newName: "PensionerPayment");

            migrationBuilder.RenameIndex(
                name: "IX_PensionerPayments_PensionerId_Month",
                table: "PensionerPayment",
                newName: "IX_PensionerPayment_PensionerId_Month");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PensionerPayment",
                table: "PensionerPayment",
                column: "PensionerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PensionerPayment_Pensioner_PensionerId",
                table: "PensionerPayment",
                column: "PensionerId",
                principalTable: "Pensioner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}