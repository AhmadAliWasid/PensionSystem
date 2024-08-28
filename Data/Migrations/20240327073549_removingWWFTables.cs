using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class removingWWFTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WWFBills");

            migrationBuilder.DropTable(
                name: "WWFPayments");

            migrationBuilder.DropTable(
                name: "WWFDemands");

            migrationBuilder.DropIndex(
                name: "IX_Relations_Name",
                table: "Relations");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Relations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Relations_Name",
                table: "Relations",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Relations_Name",
                table: "Relations");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Relations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "WWFBills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PensionerId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndingMonth = table.Column<DateTime>(type: "Date", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfMonths = table.Column<byte>(type: "tinyint", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SanctionDate = table.Column<DateTime>(type: "Date", nullable: false),
                    SanctionNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartingMonth = table.Column<DateTime>(type: "Date", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "WWFDemands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WWFDemands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WWFPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PensionerId = table.Column<int>(type: "int", nullable: false),
                    WWFDemandId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "Decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    From = table.Column<DateTime>(type: "Date", nullable: false),
                    Months = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "Decimal(18,2)", nullable: false),
                    To = table.Column<DateTime>(type: "Date", nullable: false)
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
                name: "IX_Relations_Name",
                table: "Relations",
                column: "Name",
                unique: true);

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
                name: "IX_WWFPayments_PensionerId",
                table: "WWFPayments",
                column: "PensionerId");

            migrationBuilder.CreateIndex(
                name: "IX_WWFPayments_WWFDemandId",
                table: "WWFPayments",
                column: "WWFDemandId");
        }
    }
}
