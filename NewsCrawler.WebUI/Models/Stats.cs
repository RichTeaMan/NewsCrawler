using System;

namespace NewsCrawler.WebUI.Models
{
    public class Stats
    {
        public int PostgresArticles { get; set; }

        public DateTimeOffset LatestPostgresArticle { get; set; }
    }
}
