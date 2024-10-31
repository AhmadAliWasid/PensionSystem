using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingPDUToCashBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PDUId",
                table: "CashBook",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_CashBook_PDUId",
                table: "CashBook",
                column: "PDUId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashBook_PDUs_PDUId",
                table: "CashBook",
                column: "PDUId",
                principalTable: "PDUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashBook_PDUs_PDUId",
                table: "CashBook");

            migrationBuilder.DropIndex(
                name: "IX_CashBook_PDUId",
                table: "CashBook");

            migrationBuilder.DropColumn(
                name: "PDUId",
                table: "CashBook");
        }
    }
}
