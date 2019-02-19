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
            if (string.IsNullOrWhiteSpace(articleLink))
            {
                return false;
            }
            return !Regex.IsMatch(articleLink, @"\d") && !articleLink.Contains("news/article-") && !articleLink.EndsWith(".uk") && !articleLink.EndsWith(".com");
        }
    }
}
