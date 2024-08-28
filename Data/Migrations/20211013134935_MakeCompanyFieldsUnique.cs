using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class MakeCompanyFieldsUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Company_Name",
                table: "Company",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_Order",
                table: "Company",
                column: "Order",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_ShortName",
                table: "Company",
                column: "ShortName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Company_Name",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_Order",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_ShortName",
                table: "Company");
        }
    }
}