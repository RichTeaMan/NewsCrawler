using NewsCrawler.Interfaces;
using System.Text.RegularExpressions;

namespace NewsCrawler.DailyMail
{
    public class DailyMailNewsArticleDeterminationService : INewsArticleDeterminationService
    {
        public bool IsNewsArticle(string articleLink)
        {
            bool isNews = false;
            if (!string.IsNullOrEmpty(articleLink))
            {
                isNews = articleLink.StartsWith("https://www.dailymail.co.uk/news/article-") && !articleLink.EndsWith(".rss");
            }
            return isNews;
        }

        public bool IsIndexPage(string articleLink)
        {
            if (string.IsNullOrWhiteSpace(articleLink))
            {
                return false;
            }
            return articleLink.StartsWith("https://www.dailymail.co.uk/") &&
                !Regex.IsMatch(articleLink, @"\d") &&
                !articleLink.Contains("news/article-") &&
                !articleLink.EndsWith(".rss") &&
                !articleLink.EndsWith(".uk") &&
                !articleLink.EndsWith(".com") &&
                !articleLink.EndsWith(".com/") &&
                !articleLink.Contains("twitter.com") &&
                !articleLink.Contains("digg.com") &&
                !articleLink.Contains("feedly.com") &&
                !articleLink.Contains("www.linkedin.com") &&
                !articleLink.Contains("mozilla.org") &&
                !articleLink.Contains("mailtravel.co.uk") &&
                !articleLink.Contains("wikipedia.org/") &&
                !articleLink.Contains("/breakdown-cover") &&
                !articleLink.Contains("/mortgage-finder") &&
                !articleLink.Contains("/fuel-bills-finder") &&
                !articleLink.Contains("/sharedealing") &&
                !articleLink.Contains("/creditcardfinder") &&
                !articleLink.Contains("/broadband-finder") &&
                !articleLink.Contains("/homepagemortgages");

        }
    }
}
