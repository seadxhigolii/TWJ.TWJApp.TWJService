using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class AddedpostcountforTagtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostCount",
                table: "Tag",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostCount",
                table: "Tag");
        }
    }
}
