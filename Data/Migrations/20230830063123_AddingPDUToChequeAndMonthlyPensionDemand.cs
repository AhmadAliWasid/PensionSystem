using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingPDUToChequeAndMonthlyPensionDemand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PDUId",
                table: "MonthlyPensionDemands",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "PDUId",
                table: "Cheque",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyPensionDemands_PDUId",
                table: "MonthlyPensionDemands",
                column: "PDUId");

            migrationBuilder.CreateIndex(
                name: "IX_Cheque_PDUId",
                table: "Cheque",
                column: "PDUId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheque_PDUs_PDUId",
                table: "Cheque",
                column: "PDUId",
                principalTable: "PDUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyPensionDemands_PDUs_PDUId",
                table: "MonthlyPensionDemands",
                column: "PDUId",
                principalTable: "PDUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cheque_PDUs_PDUId",
                table: "Cheque");

            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyPensionDemands_PDUs_PDUId",
                table: "MonthlyPensionDemands");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyPensionDemands_PDUId",
                table: "MonthlyPensionDemands");

            migrationBuilder.DropIndex(
                name: "IX_Cheque_PDUId",
                table: "Cheque");

            migrationBuilder.DropColumn(
                name: "PDUId",
                table: "MonthlyPensionDemands");

            migrationBuilder.DropColumn(
                name: "PDUId",
                table: "Cheque");
        }
    }
}