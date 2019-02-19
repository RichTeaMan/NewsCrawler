using NewsCrawler.Interfaces;

namespace NewsCrawler.Bbc
{
    public class BbcNewsArticleDeterminationService : INewsArticleDeterminationService
    {
        public bool IsNewsArticle(string articleLink)
        {
            return articleLink?.StartsWith("/news/") == true;
        }
    }
}
