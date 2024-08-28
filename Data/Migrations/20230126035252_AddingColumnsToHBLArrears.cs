using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingColumnsToHBLArrears : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "HBLArrears",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "HBLArrears",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_BranchId",
                table: "HBLArrears",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_HBLArrears_Branches_BranchId",
                table: "HBLArrears",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HBLArrears_Branches_BranchId",
                table: "HBLArrears");

            migrationBuilder.DropIndex(
                name: "IX_HBLArrears_BranchId",
                table: "HBLArrears");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "HBLArrears");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "HBLArrears");
        }
    }
}