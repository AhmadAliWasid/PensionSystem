using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class _20230907133513_ModifiyingColumnNameInPDU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StampThirdLine",
                table: "PDUs",
                newName: "DMStamp");

            migrationBuilder.RenameColumn(
                name: "StampSecondLine",
                table: "PDUs",
                newName: "BaseStamp");

            migrationBuilder.RenameColumn(
                name: "StampFirstLine",
                table: "PDUs",
                newName: "AMStamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DMStamp",
                table: "PDUs",
                newName: "StampThirdLine");

            migrationBuilder.RenameColumn(
                name: "BaseStamp",
                table: "PDUs",
                newName: "StampSecondLine");

            migrationBuilder.RenameColumn(
                name: "AMStamp",
                table: "PDUs",
                newName: "StampFirstLine");
        }
    }
}