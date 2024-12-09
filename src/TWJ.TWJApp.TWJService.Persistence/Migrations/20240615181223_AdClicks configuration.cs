using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class AdClicksconfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_User_UserID",
                table: "Comment");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Comment",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserID",
                table: "Comment",
                newName: "IX_Comment_UserId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Comment",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Comment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Comment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BannerId",
                table: "AdClicks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "FeaturedAdClicks",
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
                    table.PrimaryKey("PK_FeaturedAdClicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeaturedAdClicks_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeaturedAdClicks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdClicks_BannerId",
                table: "AdClicks",
                column: "BannerId");

            migrationBuilder.CreateIndex(
                name: "IX_FeaturedAdClicks_BlogPostId",
                table: "FeaturedAdClicks",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_FeaturedAdClicks_ProductId",
                table: "FeaturedAdClicks",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdClicks_Banners_BannerId",
                table: "AdClicks",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_User_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdClicks_Banners_BannerId",
                table: "AdClicks");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_User_UserId",
                table: "Comment");

            migrationBuilder.DropTable(
                name: "FeaturedAdClicks");

            migrationBuilder.DropIndex(
                name: "IX_AdClicks_BannerId",
                table: "AdClicks");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "AdClicks");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Comment",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                newName: "IX_Comment_UserID");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "Comment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_User_UserID",
                table: "Comment",
                column: "UserID",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
