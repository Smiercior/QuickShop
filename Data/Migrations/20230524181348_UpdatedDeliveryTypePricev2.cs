using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickShop.Migrations
{
    public partial class UpdatedDeliveryTypePricev2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryTypePrices_Products_ProductId",
                table: "DeliveryTypePrices");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryTypePrices_Products_ProductId",
                table: "DeliveryTypePrices",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryTypePrices_Products_ProductId",
                table: "DeliveryTypePrices");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryTypePrices_Products_ProductId",
                table: "DeliveryTypePrices",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
