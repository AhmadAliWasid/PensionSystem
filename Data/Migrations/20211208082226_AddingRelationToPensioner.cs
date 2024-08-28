using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Data.Migrations
{
    public partial class AddingRelationToPensioner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelationId",
                table: "Pensioner",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Pensioner_RelationId",
                table: "Pensioner",
                column: "RelationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pensioner_Relations_RelationId",
                table: "Pensioner",
                column: "RelationId",
                principalTable: "Relations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pensioner_Relations_RelationId",
                table: "Pensioner");

            migrationBuilder.DropIndex(
                name: "IX_Pensioner_RelationId",
                table: "Pensioner");

            migrationBuilder.DropColumn(
                name: "RelationId",
                table: "Pensioner");
        }
    }
}