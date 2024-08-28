using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class PDUToPensioner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pensioner_PDUId",
                table: "Pensioner",
                column: "PDUId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pensioner_PDUs_PDUId",
                table: "Pensioner",
                column: "PDUId",
                principalTable: "PDUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pensioner_PDUs_PDUId",
                table: "Pensioner");

            migrationBuilder.DropIndex(
                name: "IX_Pensioner_PDUId",
                table: "Pensioner");
        }
    }
}