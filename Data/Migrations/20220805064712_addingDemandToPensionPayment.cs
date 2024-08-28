using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class addingDemandToPensionPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthlyPensionDemandId",
                table: "PensionerPayments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_PensionerPayments_MonthlyPensionDemandId",
                table: "PensionerPayments",
                column: "MonthlyPensionDemandId");

            migrationBuilder.AddForeignKey(
                name: "FK_PensionerPayments_MonthlyPensionDemands_MonthlyPensionDemandId",
                table: "PensionerPayments",
                column: "MonthlyPensionDemandId",
                principalTable: "MonthlyPensionDemands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PensionerPayments_MonthlyPensionDemands_MonthlyPensionDemandId",
                table: "PensionerPayments");

            migrationBuilder.DropIndex(
                name: "IX_PensionerPayments_MonthlyPensionDemandId",
                table: "PensionerPayments");

            migrationBuilder.DropColumn(
                name: "MonthlyPensionDemandId",
                table: "PensionerPayments");
        }
    }
}