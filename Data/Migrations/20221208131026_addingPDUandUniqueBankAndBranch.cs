using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class addingPDUandUniqueBankAndBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Branches_BankId",
                table: "Branches");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Branches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "Banks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PDUs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PDUs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_BankId_Code",
                table: "Branches",
                columns: new[] { "BankId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_Banks_ShortName",
                table: "Banks",
                column: "ShortName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PDUs");

            migrationBuilder.DropIndex(
                name: "IX_Branches_BankId_Code",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Banks_ShortName",
                table: "Banks");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "Banks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_BankId",
                table: "Branches",
                column: "BankId");
        }
    }
}