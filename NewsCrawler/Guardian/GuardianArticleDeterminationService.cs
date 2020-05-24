using NewsCrawler.Interfaces;
using System.Text.RegularExpressions;

namespace NewsCrawler.Guardian
{
    public class GuardianArticleDeterminationService : INewsArticleDeterminationService
    {
        private readonly Regex newsRegex = new Regex(@"/\d{4}/\w+/\d{2}/");

        public bool IsNewsArticle(string articleLink)
        {
            bool isNewsArticle = false;
            if (!string.IsNullOrEmpty(articleLink))
            {
                isNewsArticle = articleLink.Contains("theguardian.com") &&
                    !articleLink.Contains("support.theguardian") &&
                    !articleLink.EndsWith("/all") &&
                    newsRegex.IsMatch(articleLink);
            }
            return isNewsArticle;
        }

        public bool IsIndexPage(string articleLink)
        {
            return articleLink?.Contains("https://www.theguardian.com/") == true && !IsNewsArticle(articleLink);
        }
    }
}
