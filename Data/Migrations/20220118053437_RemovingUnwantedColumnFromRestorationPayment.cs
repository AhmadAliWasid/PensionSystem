using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class RemovingUnwantedColumnFromRestorationPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestorationPayments_ArrearsDemands_ArrearsDemandId",
                table: "RestorationPayments");

            migrationBuilder.DropIndex(
                name: "IX_RestorationPayments_ArrearsDemandId",
                table: "RestorationPayments");

            migrationBuilder.DropColumn(
                name: "ArrearsDemandId",
                table: "RestorationPayments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArrearsDemandId",
                table: "RestorationPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RestorationPayments_ArrearsDemandId",
                table: "RestorationPayments",
                column: "ArrearsDemandId");

            migrationBuilder.AddForeignKey(
                name: "FK_RestorationPayments_ArrearsDemands_ArrearsDemandId",
                table: "RestorationPayments",
                column: "ArrearsDemandId",
                principalTable: "ArrearsDemands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}