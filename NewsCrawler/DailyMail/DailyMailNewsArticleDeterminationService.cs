using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System.Text.RegularExpressions;

namespace NewsCrawler.DailyMail
{
    public class DailyMailNewsArticleDeterminationService : INewsArticleDeterminationService
    {
        public bool IsNewsArticle(string articleLink)
        {
            bool isNews = false;
            if (!string.IsNullOrEmpty(articleLink)) {
                isNews = articleLink.StartsWith("/news/article-") && !articleLink.Contains("/index.rss");
            }
            return isNews;
        }

        public bool IsIndexPage(string articleLink)
        {
            if (string.IsNullOrWhiteSpace(articleLink))
            {
                return false;
            }
            return !Regex.IsMatch(articleLink, @"\d") && !articleLink.Contains("news/article-") && !articleLink.Contains("/index.rss") && !articleLink.EndsWith(".uk") && !articleLink.EndsWith(".com");
        }
    }
}
