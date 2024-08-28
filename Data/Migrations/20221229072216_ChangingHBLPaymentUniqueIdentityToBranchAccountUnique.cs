using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class ChangingHBLPaymentUniqueIdentityToBranchAccountUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pensioner_ClaimantCNIC",
                table: "Pensioner");

            migrationBuilder.DropIndex(
                name: "IX_HBLPayments_BranchId",
                table: "HBLPayments");

            migrationBuilder.DropIndex(
                name: "IX_HBLPayments_Month_AccountNumber",
                table: "HBLPayments");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimantCNIC",
                table: "Pensioner",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pensioner_ClaimantCNIC",
                table: "Pensioner",
                column: "ClaimantCNIC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HBLPayments_BranchId_AccountNumber_Month",
                table: "HBLPayments",
                columns: new[] { "BranchId", "AccountNumber", "Month" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pensioner_ClaimantCNIC",
                table: "Pensioner");

            migrationBuilder.DropIndex(
                name: "IX_HBLPayments_BranchId_AccountNumber_Month",
                table: "HBLPayments");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimantCNIC",
                table: "Pensioner",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.CreateIndex(
                name: "IX_Pensioner_ClaimantCNIC",
                table: "Pensioner",
                column: "ClaimantCNIC",
                unique: true,
                filter: "[ClaimantCNIC] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HBLPayments_BranchId",
                table: "HBLPayments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HBLPayments_Month_AccountNumber",
                table: "HBLPayments",
                columns: new[] { "Month", "AccountNumber" },
                unique: true);
        }
    }
}