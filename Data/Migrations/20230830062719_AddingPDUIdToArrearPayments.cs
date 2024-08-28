using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingPDUIdToArrearPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PDUId",
                table: "ArrearsDemands",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_ArrearsDemands_PDUId",
                table: "ArrearsDemands",
                column: "PDUId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArrearsDemands_PDUs_PDUId",
                table: "ArrearsDemands",
                column: "PDUId",
                principalTable: "PDUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArrearsDemands_PDUs_PDUId",
                table: "ArrearsDemands");

            migrationBuilder.DropIndex(
                name: "IX_ArrearsDemands_PDUId",
                table: "ArrearsDemands");

            migrationBuilder.DropColumn(
                name: "PDUId",
                table: "ArrearsDemands");
        }
    }
}