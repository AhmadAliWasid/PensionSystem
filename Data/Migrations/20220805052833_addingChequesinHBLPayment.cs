using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class addingChequesinHBLPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChequeId",
                table: "HBLPayments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_HBLPayments_ChequeId",
                table: "HBLPayments",
                column: "ChequeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HBLPayments_Cheque_ChequeId",
                table: "HBLPayments",
                column: "ChequeId",
                principalTable: "Cheque",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HBLPayments_Cheque_ChequeId",
                table: "HBLPayments");

            migrationBuilder.DropIndex(
                name: "IX_HBLPayments_ChequeId",
                table: "HBLPayments");

            migrationBuilder.DropColumn(
                name: "ChequeId",
                table: "HBLPayments");
        }
    }
}