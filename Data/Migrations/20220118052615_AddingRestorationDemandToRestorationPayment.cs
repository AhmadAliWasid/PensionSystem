using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingRestorationDemandToRestorationPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RestorationDemandId",
                table: "RestorationPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RestorationPayments_RestorationDemandId",
                table: "RestorationPayments",
                column: "RestorationDemandId");

            migrationBuilder.AddForeignKey(
                name: "FK_RestorationPayments_RestorationDemands_RestorationDemandId",
                table: "RestorationPayments",
                column: "RestorationDemandId",
                principalTable: "RestorationDemands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestorationPayments_RestorationDemands_RestorationDemandId",
                table: "RestorationPayments");

            migrationBuilder.DropIndex(
                name: "IX_RestorationPayments_RestorationDemandId",
                table: "RestorationPayments");

            migrationBuilder.DropColumn(
                name: "RestorationDemandId",
                table: "RestorationPayments");
        }
    }
}