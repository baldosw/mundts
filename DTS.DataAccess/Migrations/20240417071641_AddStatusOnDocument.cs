using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTS.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusOnDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_StatusId",
                table: "Documents",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Statuses_StatusId",
                table: "Documents",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Statuses_StatusId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_StatusId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Documents");
        }
    }
}
