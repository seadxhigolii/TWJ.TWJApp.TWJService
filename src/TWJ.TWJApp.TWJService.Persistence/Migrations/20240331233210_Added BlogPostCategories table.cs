using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class AddedBlogPostCategoriestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPost_Category_CategoryId",
                table: "BlogPost");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPost_Product_ProductID",
                table: "BlogPost");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPost_User_UserId",
                table: "BlogPost");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostSEOKeyword_BlogPost_BlogPostID",
                table: "BlogPostSEOKeyword");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostSEOKeyword_SEOKeyword_SEOKeywordID",
                table: "BlogPostSEOKeyword");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTags_BlogPost_BlogPostID",
                table: "BlogPostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BlogPost_BlogPostID",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_CategoryId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_SEOKeyword_Category_CategoryId",
                table: "SEOKeyword");

            migrationBuilder.DropForeignKey(
                name: "FK_Template_Template_ParentTemplateId",
                table: "Template");

            migrationBuilder.DropForeignKey(
                name: "FK_Template_TemplateSetting_TemplateSettingId",
                table: "Template");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateSetting",
                table: "TemplateSetting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Template",
                table: "Template");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SEOKeyword",
                table: "SEOKeyword");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Log",
                table: "Log");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPostSEOKeyword",
                table: "BlogPostSEOKeyword");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPost",
                table: "BlogPost");

            migrationBuilder.RenameTable(
                name: "TemplateSetting",
                newName: "TemplateSettings");

            migrationBuilder.RenameTable(
                name: "Template",
                newName: "Templates");

            migrationBuilder.RenameTable(
                name: "SEOKeyword",
                newName: "SEOKeywords");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "Log",
                newName: "Logs");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "ProductCategories");

            migrationBuilder.RenameTable(
                name: "BlogPostSEOKeyword",
                newName: "BlogPostSEOKeywords");

            migrationBuilder.RenameTable(
                name: "BlogPost",
                newName: "BlogPosts");

            migrationBuilder.RenameIndex(
                name: "IX_Template_TemplateSettingId",
                table: "Templates",
                newName: "IX_Templates_TemplateSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_Template_ParentTemplateId",
                table: "Templates",
                newName: "IX_Templates_ParentTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_SEOKeyword_CategoryId",
                table: "SEOKeywords",
                newName: "IX_SEOKeywords_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_CategoryId",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostSEOKeyword_SEOKeywordID",
                table: "BlogPostSEOKeywords",
                newName: "IX_BlogPostSEOKeywords_SEOKeywordID");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostSEOKeyword_BlogPostID",
                table: "BlogPostSEOKeywords",
                newName: "IX_BlogPostSEOKeywords_BlogPostID");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPost_UserId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPost_ProductID",
                table: "BlogPosts",
                newName: "IX_BlogPosts_ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPost_CategoryId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_CategoryId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "BlogPosts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "BlogPostCategoryId",
                table: "BlogPosts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductCategoryId",
                table: "BlogPosts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateSettings",
                table: "TemplateSettings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Templates",
                table: "Templates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SEOKeywords",
                table: "SEOKeywords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logs",
                table: "Logs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPostSEOKeywords",
                table: "BlogPostSEOKeywords",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPosts",
                table: "BlogPosts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BlogPostCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    URL = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_BlogPostCategoryId",
                table: "BlogPosts",
                column: "BlogPostCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_BlogPostCategories_BlogPostCategoryId",
                table: "BlogPosts",
                column: "BlogPostCategoryId",
                principalTable: "BlogPostCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_ProductCategories_CategoryId",
                table: "BlogPosts",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Products_ProductID",
                table: "BlogPosts",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_User_UserId",
                table: "BlogPosts",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostSEOKeywords_BlogPosts_BlogPostID",
                table: "BlogPostSEOKeywords",
                column: "BlogPostID",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostSEOKeywords_SEOKeywords_SEOKeywordID",
                table: "BlogPostSEOKeywords",
                column: "SEOKeywordID",
                principalTable: "SEOKeywords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTags_BlogPosts_BlogPostID",
                table: "BlogPostTags",
                column: "BlogPostID",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BlogPosts_BlogPostID",
                table: "Comment",
                column: "BlogPostID",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductCategories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SEOKeywords_ProductCategories_CategoryId",
                table: "SEOKeywords",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_Templates_ParentTemplateId",
                table: "Templates",
                column: "ParentTemplateId",
                principalTable: "Templates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_TemplateSettings_TemplateSettingId",
                table: "Templates",
                column: "TemplateSettingId",
                principalTable: "TemplateSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_BlogPostCategories_BlogPostCategoryId",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_ProductCategories_CategoryId",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Products_ProductID",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_User_UserId",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostSEOKeywords_BlogPosts_BlogPostID",
                table: "BlogPostSEOKeywords");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostSEOKeywords_SEOKeywords_SEOKeywordID",
                table: "BlogPostSEOKeywords");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTags_BlogPosts_BlogPostID",
                table: "BlogPostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BlogPosts_BlogPostID",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductCategories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_SEOKeywords_ProductCategories_CategoryId",
                table: "SEOKeywords");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_Templates_ParentTemplateId",
                table: "Templates");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_TemplateSettings_TemplateSettingId",
                table: "Templates");

            migrationBuilder.DropTable(
                name: "BlogPostCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateSettings",
                table: "TemplateSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Templates",
                table: "Templates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SEOKeywords",
                table: "SEOKeywords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategories",
                table: "ProductCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Logs",
                table: "Logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPostSEOKeywords",
                table: "BlogPostSEOKeywords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPosts",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_BlogPostCategoryId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "BlogPostCategoryId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ProductCategoryId",
                table: "BlogPosts");

            migrationBuilder.RenameTable(
                name: "TemplateSettings",
                newName: "TemplateSetting");

            migrationBuilder.RenameTable(
                name: "Templates",
                newName: "Template");

            migrationBuilder.RenameTable(
                name: "SEOKeywords",
                newName: "SEOKeyword");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                newName: "Category");

            migrationBuilder.RenameTable(
                name: "Logs",
                newName: "Log");

            migrationBuilder.RenameTable(
                name: "BlogPostSEOKeywords",
                newName: "BlogPostSEOKeyword");

            migrationBuilder.RenameTable(
                name: "BlogPosts",
                newName: "BlogPost");

            migrationBuilder.RenameIndex(
                name: "IX_Templates_TemplateSettingId",
                table: "Template",
                newName: "IX_Template_TemplateSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_Templates_ParentTemplateId",
                table: "Template",
                newName: "IX_Template_ParentTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_SEOKeywords_CategoryId",
                table: "SEOKeyword",
                newName: "IX_SEOKeyword_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                table: "Product",
                newName: "IX_Product_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostSEOKeywords_SEOKeywordID",
                table: "BlogPostSEOKeyword",
                newName: "IX_BlogPostSEOKeyword_SEOKeywordID");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostSEOKeywords_BlogPostID",
                table: "BlogPostSEOKeyword",
                newName: "IX_BlogPostSEOKeyword_BlogPostID");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_UserId",
                table: "BlogPost",
                newName: "IX_BlogPost_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_ProductID",
                table: "BlogPost",
                newName: "IX_BlogPost_ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_CategoryId",
                table: "BlogPost",
                newName: "IX_BlogPost_CategoryId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "BlogPost",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateSetting",
                table: "TemplateSetting",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Template",
                table: "Template",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SEOKeyword",
                table: "SEOKeyword",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Log",
                table: "Log",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPostSEOKeyword",
                table: "BlogPostSEOKeyword",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPost",
                table: "BlogPost",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPost_Category_CategoryId",
                table: "BlogPost",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPost_Product_ProductID",
                table: "BlogPost",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPost_User_UserId",
                table: "BlogPost",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostSEOKeyword_BlogPost_BlogPostID",
                table: "BlogPostSEOKeyword",
                column: "BlogPostID",
                principalTable: "BlogPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostSEOKeyword_SEOKeyword_SEOKeywordID",
                table: "BlogPostSEOKeyword",
                column: "SEOKeywordID",
                principalTable: "SEOKeyword",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTags_BlogPost_BlogPostID",
                table: "BlogPostTags",
                column: "BlogPostID",
                principalTable: "BlogPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BlogPost_BlogPostID",
                table: "Comment",
                column: "BlogPostID",
                principalTable: "BlogPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_CategoryId",
                table: "Product",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SEOKeyword_Category_CategoryId",
                table: "SEOKeyword",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Template_Template_ParentTemplateId",
                table: "Template",
                column: "ParentTemplateId",
                principalTable: "Template",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Template_TemplateSetting_TemplateSettingId",
                table: "Template",
                column: "TemplateSettingId",
                principalTable: "TemplateSetting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
