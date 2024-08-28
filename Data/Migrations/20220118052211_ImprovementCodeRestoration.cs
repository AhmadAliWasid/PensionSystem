using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class ImprovementCodeRestoration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ArrearsPayments");

            migrationBuilder.CreateTable(
                name: "RestorationPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PensionerId = table.Column<int>(type: "int", nullable: false),
                    FromMonth = table.Column<DateTime>(type: "Date", nullable: false),
                    ToMonth = table.Column<DateTime>(type: "Date", nullable: false),
                    NumberOfMonths = table.Column<int>(type: "int", nullable: false),
                    MP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CMA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Orderly = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPension = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Month = table.Column<DateTime>(type: "Date", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrearsDemandId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestorationPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestorationPayments_ArrearsDemands_ArrearsDemandId",
                        column: x => x.ArrearsDemandId,
                        principalTable: "ArrearsDemands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestorationPayments_Pensioner_PensionerId",
                        column: x => x.PensionerId,
                        principalTable: "Pensioner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestorationPayments_ArrearsDemandId",
                table: "RestorationPayments",
                column: "ArrearsDemandId");

            migrationBuilder.CreateIndex(
                name: "IX_RestorationPayments_PensionerId",
                table: "RestorationPayments",
                column: "PensionerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestorationPayments");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ArrearsPayments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}