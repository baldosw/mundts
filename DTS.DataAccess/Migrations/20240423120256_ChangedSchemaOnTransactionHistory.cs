using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTS.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedSchemaOnTransactionHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "DocumentTitle",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "ModifiedName",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "RouteDepartment",
                table: "TransactionHistories");

            migrationBuilder.RenameColumn(
                name: "StatusTitle",
                table: "TransactionHistories",
                newName: "TrackingCode");

            migrationBuilder.RenameColumn(
                name: "SourceDepartment",
                table: "TransactionHistories",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "TransactionHistories",
                newName: "RequestTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "TransactionHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "TransactionHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteDepartmentId",
                table: "TransactionHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "TransactionHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_DepartmentId",
                table: "TransactionHistories",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_RequestTypeId",
                table: "TransactionHistories",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_StatusId",
                table: "TransactionHistories",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Departments_DepartmentId",
                table: "TransactionHistories",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_RequestTypes_RequestTypeId",
                table: "TransactionHistories",
                column: "RequestTypeId",
                principalTable: "RequestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Statuses_StatusId",
                table: "TransactionHistories",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Departments_DepartmentId",
                table: "TransactionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_RequestTypes_RequestTypeId",
                table: "TransactionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Statuses_StatusId",
                table: "TransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistories_DepartmentId",
                table: "TransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistories_RequestTypeId",
                table: "TransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistories_StatusId",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "RouteDepartmentId",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "TransactionHistories");

            migrationBuilder.RenameColumn(
                name: "TrackingCode",
                table: "TransactionHistories",
                newName: "StatusTitle");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "TransactionHistories",
                newName: "SourceDepartment");

            migrationBuilder.RenameColumn(
                name: "RequestTypeId",
                table: "TransactionHistories",
                newName: "EmployeeId");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "TransactionHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "TransactionHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentTitle",
                table: "TransactionHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedName",
                table: "TransactionHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RouteDepartment",
                table: "TransactionHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
