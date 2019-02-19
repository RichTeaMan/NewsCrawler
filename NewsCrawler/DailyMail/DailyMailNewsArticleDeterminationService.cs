using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System.Text.RegularExpressions;

namespace NewsCrawler.DailyMail
{
    public class DailyMailNewsArticleDeterminationService : INewsArticleDeterminationService
    {
        public bool IsNewsArticle(string articleLink)
        {
            return articleLink?.StartsWith("/news/article-") == true;
        }

        public bool IsIndexPage(string articleLink)
        {
            return !Regex.IsMatch(articleLink, @"\d") && !articleLink.Contains("news/article-");
        }
    }
}
