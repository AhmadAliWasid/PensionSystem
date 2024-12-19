using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingChequeDetailToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ChequeDate",
                table: "WGReimbursments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "ChequeNo",
                table: "WGReimbursments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsReimbursed",
                table: "WGReimbursments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChequeDate",
                table: "WGReimbursments");

            migrationBuilder.DropColumn(
                name: "ChequeNo",
                table: "WGReimbursments");

            migrationBuilder.DropColumn(
                name: "IsReimbursed",
                table: "WGReimbursments");
        }
    }
}
