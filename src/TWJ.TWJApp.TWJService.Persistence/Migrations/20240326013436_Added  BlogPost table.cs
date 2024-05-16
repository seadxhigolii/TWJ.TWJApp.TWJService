using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class AddedBlogPosttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "BlogPost",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Published",
                table: "BlogPost");
        }
    }
}
