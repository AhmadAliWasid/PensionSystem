using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyingHBLArrearsBankListUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HBLArrears_PensionerId_FromMonth_ChequeId",
                table: "HBLArrears");

            migrationBuilder.DropIndex(
                name: "IX_HBLArrears_PensionerId_ToMonth_ChequeId",
                table: "HBLArrears");

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_PensionerId_FromMonth_ChequeId_Description",
                table: "HBLArrears",
                columns: new[] { "PensionerId", "FromMonth", "ChequeId", "Description" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_PensionerId_ToMonth_ChequeId_Description",
                table: "HBLArrears",
                columns: new[] { "PensionerId", "ToMonth", "ChequeId", "Description" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HBLArrears_PensionerId_FromMonth_ChequeId_Description",
                table: "HBLArrears");

            migrationBuilder.DropIndex(
                name: "IX_HBLArrears_PensionerId_ToMonth_ChequeId_Description",
                table: "HBLArrears");

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_PensionerId_FromMonth_ChequeId",
                table: "HBLArrears",
                columns: new[] { "PensionerId", "FromMonth", "ChequeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HBLArrears_PensionerId_ToMonth_ChequeId",
                table: "HBLArrears",
                columns: new[] { "PensionerId", "ToMonth", "ChequeId" },
                unique: true);
        }
    }
}