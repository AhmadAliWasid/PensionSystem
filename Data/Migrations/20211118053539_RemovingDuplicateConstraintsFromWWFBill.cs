using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class RemovingDuplicateConstraintsFromWWFBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WWFBills_SanctionNumber_SanctionDate",
                table: "WWFBills");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WWFBills_SanctionNumber_SanctionDate",
                table: "WWFBills",
                columns: new[] { "SanctionNumber", "SanctionDate" },
                unique: true);
        }
    }
}