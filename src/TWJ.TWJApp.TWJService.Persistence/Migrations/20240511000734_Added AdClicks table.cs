using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class AddedAdClickstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdClicks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlogPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClickTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserSessionId = table.Column<string>(type: "text", nullable: true),
                    Converted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdClicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdClicks_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdClicks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdClicks_BlogPostId",
                table: "AdClicks",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_AdClicks_ProductId",
                table: "AdClicks",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdClicks");
        }
    }
}
