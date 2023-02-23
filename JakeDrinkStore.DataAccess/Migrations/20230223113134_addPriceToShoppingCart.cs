using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JakeDrinkStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addPriceToShoppingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CasePrice",
                table: "ShoppingCarts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "IndividualPrice",
                table: "ShoppingCarts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CasePrice",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "IndividualPrice",
                table: "ShoppingCarts");
        }
    }
}
