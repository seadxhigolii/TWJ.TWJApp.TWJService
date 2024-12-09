using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class AddedCategoryIDtoSEOKeywordtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "SEOKeyword",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SEOKeyword_CategoryId",
                table: "SEOKeyword",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SEOKeyword_Category_CategoryId",
                table: "SEOKeyword",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SEOKeyword_Category_CategoryId",
                table: "SEOKeyword");

            migrationBuilder.DropIndex(
                name: "IX_SEOKeyword_CategoryId",
                table: "SEOKeyword");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "SEOKeyword");
        }
    }
}
