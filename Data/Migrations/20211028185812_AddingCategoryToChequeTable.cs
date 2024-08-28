using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingCategoryToChequeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChequeCategoryId",
                table: "Cheque",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cheque_ChequeCategoryId",
                table: "Cheque",
                column: "ChequeCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheque_ChequeCategory_ChequeCategoryId",
                table: "Cheque",
                column: "ChequeCategoryId",
                principalTable: "ChequeCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cheque_ChequeCategory_ChequeCategoryId",
                table: "Cheque");

            migrationBuilder.DropIndex(
                name: "IX_Cheque_ChequeCategoryId",
                table: "Cheque");

            migrationBuilder.DropColumn(
                name: "ChequeCategoryId",
                table: "Cheque");
        }
    }
}