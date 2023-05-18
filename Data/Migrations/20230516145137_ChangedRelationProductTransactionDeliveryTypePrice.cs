using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickShop.Migrations
{
    public partial class ChangedRelationProductTransactionDeliveryTypePrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransactions_DeliveryTypePrices_DeliveryTypePriceId",
                table: "ProductTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProductTransactions_DeliveryTypePriceId",
                table: "ProductTransactions");

            migrationBuilder.DropColumn(
                name: "DeliveryTypePriceId",
                table: "ProductTransactions");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTypePriceId",
                table: "ProductTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransactions_DeliveryTypePriceId",
                table: "ProductTransactions",
                column: "DeliveryTypePriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransactions_DeliveryTypePrices_DeliveryTypePriceId",
                table: "ProductTransactions",
                column: "DeliveryTypePriceId",
                principalTable: "DeliveryTypePrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
