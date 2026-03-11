using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareLicenses.Migrations
{
    /// <inheritdoc />
    public partial class AddInstalledByEmployeeToInstallation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstalledByEmployeeId",
                table: "Installations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Installations_InstalledByEmployeeId",
                table: "Installations",
                column: "InstalledByEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Installations_Employees_InstalledByEmployeeId",
                table: "Installations",
                column: "InstalledByEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Installations_Employees_InstalledByEmployeeId",
                table: "Installations");

            migrationBuilder.DropIndex(
                name: "IX_Installations_InstalledByEmployeeId",
                table: "Installations");

            migrationBuilder.DropColumn(
                name: "InstalledByEmployeeId",
                table: "Installations");
        }
    }
}
