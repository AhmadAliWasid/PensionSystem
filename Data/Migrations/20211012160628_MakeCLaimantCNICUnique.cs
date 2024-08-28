using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class MakeCLaimantCNICUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pensioner_ClaimantCNIC",
                table: "Pensioner",
                column: "ClaimantCNIC",
                unique: true,
                filter: "[ClaimantCNIC] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pensioner_ClaimantCNIC",
                table: "Pensioner");
        }
    }
}