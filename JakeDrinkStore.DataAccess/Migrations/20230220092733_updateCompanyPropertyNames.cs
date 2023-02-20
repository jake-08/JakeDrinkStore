using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JakeDrinkStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateCompanyPropertyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Companies",
                newName: "Suburb");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Companies",
                newName: "Postcode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Suburb",
                table: "Companies",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "Postcode",
                table: "Companies",
                newName: "City");
        }
    }
}
