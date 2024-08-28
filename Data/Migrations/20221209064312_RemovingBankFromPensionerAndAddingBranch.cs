using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class RemovingBankFromPensionerAndAddingBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pensioner_Banks_BankId",
                table: "Pensioner");

            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "Pensioner",
                newName: "BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_Pensioner_BankId",
                table: "Pensioner",
                newName: "IX_Pensioner_BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pensioner_Branches_BranchId",
                table: "Pensioner",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pensioner_Branches_BranchId",
                table: "Pensioner");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "Pensioner",
                newName: "BankId");

            migrationBuilder.RenameIndex(
                name: "IX_Pensioner_BranchId",
                table: "Pensioner",
                newName: "IX_Pensioner_BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pensioner_Banks_BankId",
                table: "Pensioner",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}