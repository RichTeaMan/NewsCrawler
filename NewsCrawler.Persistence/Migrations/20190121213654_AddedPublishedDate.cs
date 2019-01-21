using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsCrawler.Persistence.Migrations
{
    public partial class AddedPublishedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PublishedDate",
                table: "Articles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Articles");
        }
    }
}
