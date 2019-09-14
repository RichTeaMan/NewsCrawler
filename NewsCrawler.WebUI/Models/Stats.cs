using System;

namespace NewsCrawler.WebUI.Models
{
    public class Stats
    {
        public int PostgresArticles { get; set; }

        public int SqlServerArticles { get; set; }

        public DateTimeOffset LatestPostgresArticle { get; set; }

        public DateTimeOffset LatestSqlServerArticle { get; set; }
    }
}
