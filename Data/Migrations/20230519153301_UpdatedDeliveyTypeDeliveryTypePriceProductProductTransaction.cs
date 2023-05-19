using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickShop.Migrations
{
    public partial class UpdatedDeliveyTypeDeliveryTypePriceProductProductTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransactions_DeliveryTypes_DeliveryTypeId",
                table: "ProductTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProductTransactions_DeliveryTypeId",
                table: "ProductTransactions");

            migrationBuilder.DropColumn(
                name: "DeliveryTypeId",
                table: "ProductTransactions");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTypePriceId",
                table: "ProductTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParametersLink",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "DeliveryTypePrices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransactions_DeliveryTypePriceId",
                table: "ProductTransactions",
                column: "DeliveryTypePriceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTypePrices_ProductId",
                table: "DeliveryTypePrices",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryTypePrices_Products_ProductId",
                table: "DeliveryTypePrices",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransactions_DeliveryTypePrices_DeliveryTypePriceId",
                table: "ProductTransactions",
                column: "DeliveryTypePriceId",
                principalTable: "DeliveryTypePrices",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryTypePrices_Products_ProductId",
                table: "DeliveryTypePrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTransactions_DeliveryTypePrices_DeliveryTypePriceId",
                table: "ProductTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProductTransactions_DeliveryTypePriceId",
                table: "ProductTransactions");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryTypePrices_ProductId",
                table: "DeliveryTypePrices");

            migrationBuilder.DropColumn(
                name: "DeliveryTypePriceId",
                table: "ProductTransactions");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "DeliveryTypePrices");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTypeId",
                table: "ProductTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ParametersLink",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransactions_DeliveryTypeId",
                table: "ProductTransactions",
                column: "DeliveryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTransactions_DeliveryTypes_DeliveryTypeId",
                table: "ProductTransactions",
                column: "DeliveryTypeId",
                principalTable: "DeliveryTypes",
                principalColumn: "Id");
        }
    }
}
