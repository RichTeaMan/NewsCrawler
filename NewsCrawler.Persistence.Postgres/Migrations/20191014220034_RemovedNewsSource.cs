using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsCrawler.Persistence.Postgres.Migrations
{
    public partial class RemovedNewsSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewsSource",
                table: "Articles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewsSource",
                table: "Articles",
                maxLength: 20,
                nullable: false,
                defaultValue: "Unspecified");
        }
    }
}
