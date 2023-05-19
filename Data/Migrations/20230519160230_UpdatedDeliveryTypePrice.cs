using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickShop.Migrations
{
    public partial class UpdatedDeliveryTypePrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryTypePrices_ProductTransactions_ProductTransactionId",
                table: "DeliveryTypePrices");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryTypePrices_ProductTransactionId",
                table: "DeliveryTypePrices");

            migrationBuilder.DropColumn(
                name: "ProductTransactionId",
                table: "DeliveryTypePrices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductTransactionId",
                table: "DeliveryTypePrices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTypePrices_ProductTransactionId",
                table: "DeliveryTypePrices",
                column: "ProductTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryTypePrices_ProductTransactions_ProductTransactionId",
                table: "DeliveryTypePrices",
                column: "ProductTransactionId",
                principalTable: "ProductTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
