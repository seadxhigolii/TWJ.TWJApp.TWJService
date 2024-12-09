using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class AddednewcolumnsforBlogPosttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackLinkKeywords",
                table: "BlogPost",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "BlogPost",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackLinkKeywords",
                table: "BlogPost");

            migrationBuilder.DropColumn(
                name: "URL",
                table: "BlogPost");
        }
    }
}
