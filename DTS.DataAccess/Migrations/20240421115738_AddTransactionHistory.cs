using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTS.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    DocumentTitle = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(700)", nullable: false),
                    StatusTitle = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(700)", nullable: true),
                    SourceDepartment = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    RouteDepartment = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ModifiedName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionHistories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionHistories");
        }
    }
}
