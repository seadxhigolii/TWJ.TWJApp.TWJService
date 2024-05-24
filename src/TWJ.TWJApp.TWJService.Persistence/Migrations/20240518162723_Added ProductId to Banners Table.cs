using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class AddedProductIdtoBannersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            var defaultProductId = new Guid("e41553f5-be5d-43ae-bf15-67eca10061fe");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Banners",
                type: "uuid",
                nullable: false,
                defaultValue: defaultProductId);

            migrationBuilder.CreateIndex(
                name: "IX_Banners_ProductId",
                table: "Banners",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Products_ProductId",
                table: "Banners",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Products_ProductId",
                table: "Banners");

            migrationBuilder.DropIndex(
                name: "IX_Banners_ProductId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Banners");
        }
    }

}
