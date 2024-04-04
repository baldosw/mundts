using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTS.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStatusTitleAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusTitle",
                table: "Statuses",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "StatusDescription",
                table: "Statuses",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Statuses",
                newName: "StatusTitle");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Statuses",
                newName: "StatusDescription");
        }
    }
}
