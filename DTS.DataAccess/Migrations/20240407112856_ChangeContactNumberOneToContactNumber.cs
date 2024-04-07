using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTS.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeContactNumberOneToContactNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactNumberOne",
                table: "Employees",
                newName: "ContactNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactNumber",
                table: "Employees",
                newName: "ContactNumberOne");
        }
    }
}
