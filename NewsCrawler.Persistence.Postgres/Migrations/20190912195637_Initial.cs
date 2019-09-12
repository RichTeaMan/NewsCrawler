using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NewsCrawler.Persistence.Postgres.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Url = table.Column<string>(maxLength: 400, nullable: false),
                    Title = table.Column<string>(maxLength: 250, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ContentLength = table.Column<int>(nullable: false),
                    CleanedContent = table.Column<string>(type: "text", nullable: true),
                    CleanedContentLength = table.Column<int>(nullable: false),
                    RecordedDate = table.Column<DateTimeOffset>(nullable: false),
                    PublishedDate = table.Column<DateTimeOffset>(nullable: true),
                    IsIndexPage = table.Column<bool>(nullable: false),
                    NewsSource = table.Column<string>(maxLength: 20, nullable: false, defaultValue: "Unspecified")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WordCount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Word = table.Column<string>(maxLength: 30, nullable: false),
                    Count = table.Column<int>(nullable: false),
                    NewsSource = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordCount", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "WordCount");
        }
    }
}
