using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    public partial class addedselfrelationshipofTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ParentId",
                table: "Template",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentTemplateId",
                table: "Template",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Template_ParentTemplateId",
                table: "Template",
                column: "ParentTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Template_Template_ParentTemplateId",
                table: "Template",
                column: "ParentTemplateId",
                principalTable: "Template",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Template_Template_ParentTemplateId",
                table: "Template");

            migrationBuilder.DropIndex(
                name: "IX_Template_ParentTemplateId",
                table: "Template");

            migrationBuilder.DropColumn(
                name: "ParentTemplateId",
                table: "Template");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "Template",
                type: "integer",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
