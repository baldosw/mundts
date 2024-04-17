using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTS.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteDepartmentIdToDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RouteDepartmentId",
                table: "Documents",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RouteDepartmentId",
                table: "Documents");
        }
    }
}
