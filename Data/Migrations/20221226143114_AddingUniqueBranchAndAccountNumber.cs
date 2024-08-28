using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingUniqueBranchAndAccountNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pensioner_AccountNumber",
                table: "Pensioner");

            migrationBuilder.CreateIndex(
                name: "IX_Pensioner_AccountNumber_BranchId",
                table: "Pensioner",
                columns: new[] { "AccountNumber", "BranchId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pensioner_AccountNumber_BranchId",
                table: "Pensioner");

            migrationBuilder.CreateIndex(
                name: "IX_Pensioner_AccountNumber",
                table: "Pensioner",
                column: "AccountNumber",
                unique: true);
        }
    }
}