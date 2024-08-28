using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingTableArrearsPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArrearsPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PensionerId = table.Column<int>(type: "int", nullable: false),
                    FromMonth = table.Column<DateTime>(type: "Date", nullable: false),
                    ToMonth = table.Column<DateTime>(type: "Date", nullable: false),
                    MP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CMA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Orderly = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPension = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Month = table.Column<DateTime>(type: "Date", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrearsPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArrearsPayments_Pensioner_PensionerId",
                        column: x => x.PensionerId,
                        principalTable: "Pensioner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArrearsPayments_PensionerId",
                table: "ArrearsPayments",
                column: "PensionerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArrearsPayments");
        }
    }
}