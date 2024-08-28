using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingHBLArearsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HBLArrears",
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
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HBLArrears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HBLArrears_Cheque_ChequeId",
                        column: x => x.ChequeId,
                        principalTable: "Cheque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HBLArrears_Pensioner_PensionerId",
                        column: x => x.PensionerId,
                        principalTable: "Pensioner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_ChequeId",
                table: "HBLArrears",
                column: "ChequeId");

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_PensionerId_FromMonth",
                table: "HBLArrears",
                columns: new[] { "PensionerId", "FromMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_PensionerId_ToMonth",
                table: "HBLArrears",
                columns: new[] { "PensionerId", "ToMonth" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HBLArrears");
        }
    }
}