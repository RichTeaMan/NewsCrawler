using NewsCrawler.Interfaces;

namespace NewsCrawler.DailyMail
{
    public class DailyMailNewsArticleDeterminationService : INewsArticleDeterminationService
    {
        public bool IsNewsArticle(string articleLink)
        {
            return articleLink?.StartsWith("/news/article-") == true;
        }
    }
}
