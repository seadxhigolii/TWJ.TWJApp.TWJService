using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class AddedRelationbetweenBlogPoststableandBlogPostCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add a default category (if not existing)
            migrationBuilder.InsertData(
                table: "BlogPostCategories",
                columns: new[] { "Id", "Name", "Description", "CreatedAt" },
                values: new object[] {
                    new Guid("00000000-0000-0000-0000-000000000000"),
                    "Uncategorized",
                    "Posts that have not been categorized",
                    DateTime.UtcNow // Assuming UTC; adjust as necessary for your application's time zone
                });

            // Step 2: Update existing BlogPosts to use the default category
            migrationBuilder.Sql("UPDATE \"BlogPosts\" SET \"BlogPostCategoryId\" = '00000000-0000-0000-0000-000000000000' WHERE \"BlogPostCategoryId\" IS NULL;");

            // Your existing migration code
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_BlogPostCategories_BlogPostCategoryId",
                table: "BlogPosts");

            migrationBuilder.AlterColumn<Guid>(
                name: "BlogPostCategoryId",
                table: "BlogPosts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_BlogPostCategories_BlogPostCategoryId",
                table: "BlogPosts",
                column: "BlogPostCategoryId",
                principalTable: "BlogPostCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_BlogPostCategories_BlogPostCategoryId",
                table: "BlogPosts");

            migrationBuilder.AlterColumn<Guid>(
                name: "BlogPostCategoryId",
                table: "BlogPosts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_BlogPostCategories_BlogPostCategoryId",
                table: "BlogPosts",
                column: "BlogPostCategoryId",
                principalTable: "BlogPostCategories",
                principalColumn: "Id");
        }
    }
}
