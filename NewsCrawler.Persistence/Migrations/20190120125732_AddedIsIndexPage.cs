using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsCrawler.Persistence.Migrations
{
    public partial class AddedIsIndexPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsIndexPage",
                table: "Articles",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIndexPage",
                table: "Articles");
        }
    }
}
