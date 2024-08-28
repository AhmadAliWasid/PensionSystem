using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingTableWWFPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WWFPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PensionerId = table.Column<int>(type: "int", nullable: false),
                    WWFDemandId = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<DateTime>(type: "Date", nullable: false),
                    To = table.Column<DateTime>(type: "Date", nullable: false),
                    Months = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "Decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "Decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WWFPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WWFPayments_Pensioner_PensionerId",
                        column: x => x.PensionerId,
                        principalTable: "Pensioner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WWFPayments_WWFDemands_WWFDemandId",
                        column: x => x.WWFDemandId,
                        principalTable: "WWFDemands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WWFPayments_PensionerId",
                table: "WWFPayments",
                column: "PensionerId");

            migrationBuilder.CreateIndex(
                name: "IX_WWFPayments_WWFDemandId",
                table: "WWFPayments",
                column: "WWFDemandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WWFPayments");
        }
    }
}