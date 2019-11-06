using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NewsCrawler.Persistence.Postgres.Migrations
{
    public partial class AddedContentTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticleCleanedContentId",
                table: "Articles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArticleContentId",
                table: "Articles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTransferred",
                table: "Articles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ArticleCleanedContent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CleanedContent = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCleanedContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleContent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleContent", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleCleanedContentId",
                table: "Articles",
                column: "ArticleCleanedContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleContentId",
                table: "Articles",
                column: "ArticleContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleCleanedContent_ArticleCleanedContentId",
                table: "Articles",
                column: "ArticleCleanedContentId",
                principalTable: "ArticleCleanedContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleContent_ArticleContentId",
                table: "Articles",
                column: "ArticleContentId",
                principalTable: "ArticleContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleCleanedContent_ArticleCleanedContentId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleContent_ArticleContentId",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "ArticleCleanedContent");

            migrationBuilder.DropTable(
                name: "ArticleContent");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ArticleCleanedContentId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ArticleContentId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleCleanedContentId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleContentId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "IsTransferred",
                table: "Articles");
        }
    }
}
