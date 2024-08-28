using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class CreatingWWFBillTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WWFBills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PensionerId = table.Column<int>(type: "int", nullable: false),
                    SanctionNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SanctionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartingMonth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndingMonth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfMonths = table.Column<byte>(type: "tinyint", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WWFBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WWFBills_Pensioner_PensionerId",
                        column: x => x.PensionerId,
                        principalTable: "Pensioner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WWFBills_PensionerId_EndingMonth",
                table: "WWFBills",
                columns: new[] { "PensionerId", "EndingMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WWFBills_PensionerId_StartingMonth",
                table: "WWFBills",
                columns: new[] { "PensionerId", "StartingMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WWFBills_SanctionNumber_SanctionDate",
                table: "WWFBills",
                columns: new[] { "SanctionNumber", "SanctionDate" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WWFBills");
        }
    }
}