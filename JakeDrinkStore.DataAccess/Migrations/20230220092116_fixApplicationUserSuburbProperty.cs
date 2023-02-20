using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JakeDrinkStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fixApplicationUserSuburbProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Surburb",
                table: "AspNetUsers",
                newName: "Suburb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Suburb",
                table: "AspNetUsers",
                newName: "Surburb");
        }
    }
}
