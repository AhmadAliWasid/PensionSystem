using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class ImprovingHBLRestoration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "HBLArrears");

            migrationBuilder.CreateTable(
                name: "HBLRestorations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChequeId = table.Column<int>(type: "int", nullable: false),
                    PensionerId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<DateTime>(type: "Date", nullable: false),
                    FromMonth = table.Column<DateTime>(type: "Date", nullable: false),
                    ToMonth = table.Column<DateTime>(type: "Date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HBLRestorations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HBLRestorations_Cheque_ChequeId",
                        column: x => x.ChequeId,
                        principalTable: "Cheque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HBLRestorations_Pensioner_PensionerId",
                        column: x => x.PensionerId,
                        principalTable: "Pensioner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HBLRestorations_ChequeId",
                table: "HBLRestorations",
                column: "ChequeId");

            migrationBuilder.CreateIndex(
                name: "IX_HBLRestorations_PensionerId",
                table: "HBLRestorations",
                column: "PensionerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HBLRestorations");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "HBLArrears",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}