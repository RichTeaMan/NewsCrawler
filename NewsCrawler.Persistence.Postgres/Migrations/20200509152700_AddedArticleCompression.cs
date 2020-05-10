using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsCrawler.Persistence.Postgres.Migrations
{
    public partial class AddedArticleCompression : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ArticleContent",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<byte[]>(
                name: "CompressedContent",
                table: "ArticleContent",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompressionType",
                table: "ArticleContent",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompressedContent",
                table: "ArticleContent");

            migrationBuilder.DropColumn(
                name: "CompressionType",
                table: "ArticleContent");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ArticleContent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
