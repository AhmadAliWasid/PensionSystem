using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingHBLArrearsToCheque : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HBLArrears_PensionerId_ToMonth",
                table: "HBLArrears");

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_PensionerId_ToMonth_ChequeId",
                table: "HBLArrears",
                columns: new[] { "PensionerId", "ToMonth", "ChequeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HBLArrears_PensionerId_ToMonth_ChequeId",
                table: "HBLArrears");

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_PensionerId_ToMonth",
                table: "HBLArrears",
                columns: new[] { "PensionerId", "ToMonth" },
                unique: true);
        }
    }
}