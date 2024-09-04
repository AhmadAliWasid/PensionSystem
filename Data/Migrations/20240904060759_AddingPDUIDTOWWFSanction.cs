using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingPDUIDTOWWFSanction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PDUId",
                table: "WWFSanctions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WWFSanctions_PDUId",
                table: "WWFSanctions",
                column: "PDUId");

            migrationBuilder.AddForeignKey(
                name: "FK_WWFSanctions_PDUs_PDUId",
                table: "WWFSanctions",
                column: "PDUId",
                principalTable: "PDUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WWFSanctions_PDUs_PDUId",
                table: "WWFSanctions");

            migrationBuilder.DropIndex(
                name: "IX_WWFSanctions_PDUId",
                table: "WWFSanctions");

            migrationBuilder.DropColumn(
                name: "PDUId",
                table: "WWFSanctions");
        }
    }
}
