using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingChequeToCommutation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChequeId",
                table: "Commutations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Commutations_ChequeId",
                table: "Commutations",
                column: "ChequeId");

            migrationBuilder.CreateIndex(
                name: "IX_Commutations_PensionerId",
                table: "Commutations",
                column: "PensionerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commutations_Cheque_ChequeId",
                table: "Commutations",
                column: "ChequeId",
                principalTable: "Cheque",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Commutations_Pensioner_PensionerId",
                table: "Commutations",
                column: "PensionerId",
                principalTable: "Pensioner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commutations_Cheque_ChequeId",
                table: "Commutations");

            migrationBuilder.DropForeignKey(
                name: "FK_Commutations_Pensioner_PensionerId",
                table: "Commutations");

            migrationBuilder.DropIndex(
                name: "IX_Commutations_ChequeId",
                table: "Commutations");

            migrationBuilder.DropIndex(
                name: "IX_Commutations_PensionerId",
                table: "Commutations");

            migrationBuilder.DropColumn(
                name: "ChequeId",
                table: "Commutations");
        }
    }
}