using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTS.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestTypeOnDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestTypeId",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_RequestTypeId",
                table: "Documents",
                column: "RequestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_RequestTypes_RequestTypeId",
                table: "Documents",
                column: "RequestTypeId",
                principalTable: "RequestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_RequestTypes_RequestTypeId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_RequestTypeId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "RequestTypeId",
                table: "Documents");
        }
    }
}
