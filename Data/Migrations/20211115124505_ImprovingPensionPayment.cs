using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class ImprovingPensionPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PensionerPayments",
                table: "PensionerPayments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PensionerPayments",
                table: "PensionerPayments",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PensionerPayments",
                table: "PensionerPayments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PensionerPayments",
                table: "PensionerPayments",
                column: "PensionerId");
        }
    }
}