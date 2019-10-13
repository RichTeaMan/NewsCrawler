using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System.Text.RegularExpressions;

namespace NewsCrawler.NewYorkTimes
{
    public class NewYorkTimesArticleDeterminationService : INewsArticleDeterminationService
    {
        private readonly Regex newsRegex = new Regex(@"https://www.nytimes.com/\d{4}/\w+/\d{2}/");

        public bool IsNewsArticle(string articleLink)
        {
            bool isNewsArticle = false;
            if (!string.IsNullOrEmpty(articleLink))
            {
                isNewsArticle = newsRegex.IsMatch(articleLink);
            }
            return isNewsArticle;
        }

        public bool IsIndexPage(string articleLink)
        {
            return articleLink != null && (articleLink.StartsWith("https://www.nytimes.com/section/") || articleLink.StartsWith("https://www.nytimes.com/column/"));
        }
    }
}
