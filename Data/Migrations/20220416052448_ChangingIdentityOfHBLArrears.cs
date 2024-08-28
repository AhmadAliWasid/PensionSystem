using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class ChangingIdentityOfHBLArrears : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HBLArrears_PensionerId_FromMonth",
                table: "HBLArrears");

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_PensionerId_FromMonth_ChequeId",
                table: "HBLArrears",
                columns: new[] { "PensionerId", "FromMonth", "ChequeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HBLArrears_PensionerId_FromMonth_ChequeId",
                table: "HBLArrears");

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_PensionerId_FromMonth",
                table: "HBLArrears",
                columns: new[] { "PensionerId", "FromMonth" },
                unique: true);
        }
    }
}