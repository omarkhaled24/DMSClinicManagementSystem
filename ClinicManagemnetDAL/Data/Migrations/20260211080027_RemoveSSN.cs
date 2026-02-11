using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagemnetDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSSN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SSN",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Clinics",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Clinics",
                newName: "ID");

            migrationBuilder.AddColumn<string>(
                name: "SSN",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
