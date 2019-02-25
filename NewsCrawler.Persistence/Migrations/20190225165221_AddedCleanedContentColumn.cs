using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsCrawler.Persistence.Migrations
{
    public partial class AddedCleanedContentColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CleanedContent",
                table: "Articles",
                type: "ntext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CleanedContentLength",
                table: "Articles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContentLength",
                table: "Articles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CleanedContent",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CleanedContentLength",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ContentLength",
                table: "Articles");
        }
    }
}
