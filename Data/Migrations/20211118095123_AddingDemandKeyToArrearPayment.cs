using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingDemandKeyToArrearPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArrearsDemandId",
                table: "ArrearsPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ArrearsPayments_ArrearsDemandId",
                table: "ArrearsPayments",
                column: "ArrearsDemandId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArrearsPayments_ArrearsDemands_ArrearsDemandId",
                table: "ArrearsPayments",
                column: "ArrearsDemandId",
                principalTable: "ArrearsDemands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArrearsPayments_ArrearsDemands_ArrearsDemandId",
                table: "ArrearsPayments");

            migrationBuilder.DropIndex(
                name: "IX_ArrearsPayments_ArrearsDemandId",
                table: "ArrearsPayments");

            migrationBuilder.DropColumn(
                name: "ArrearsDemandId",
                table: "ArrearsPayments");
        }
    }
}