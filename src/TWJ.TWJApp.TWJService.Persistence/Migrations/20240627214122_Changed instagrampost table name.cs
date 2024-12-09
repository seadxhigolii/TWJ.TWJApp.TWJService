using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class Changedinstagramposttablename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InstagramPost",
                table: "InstagramPost");

            migrationBuilder.RenameTable(
                name: "InstagramPost",
                newName: "InstagramPosts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstagramPosts",
                table: "InstagramPosts",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InstagramPosts",
                table: "InstagramPosts");

            migrationBuilder.RenameTable(
                name: "InstagramPosts",
                newName: "InstagramPost");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstagramPost",
                table: "InstagramPost",
                column: "Id");
        }
    }
}
