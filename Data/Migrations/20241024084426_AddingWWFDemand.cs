using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingWWFDemand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WWFDemands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "Date", nullable: false),
                    WWFSanctionId = table.Column<int>(type: "int", nullable: false),
                    PDUId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WWFDemands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WWFDemands_PDUs_PDUId",
                        column: x => x.PDUId,
                        principalTable: "PDUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_WWFDemands_WWFSanctions_WWFSanctionId",
                        column: x => x.WWFSanctionId,
                        principalTable: "WWFSanctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WWFDemands_PDUId",
                table: "WWFDemands",
                column: "PDUId");

            migrationBuilder.CreateIndex(
                name: "IX_WWFDemands_WWFSanctionId",
                table: "WWFDemands",
                column: "WWFSanctionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WWFDemands");
        }
    }
}
