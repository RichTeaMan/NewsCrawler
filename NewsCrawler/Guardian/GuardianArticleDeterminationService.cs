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
                isNewsArticle = !articleLink.Contains("support.theguardian") && newsRegex.IsMatch(articleLink);
            }
            return isNewsArticle;
        }

        public bool IsIndexPage(string articleLink)
        {
            if (!string.IsNullOrEmpty(articleLink))
            {
                return articleLink.Contains("https://www.theguardian.com/") && !IsNewsArticle(articleLink);
            }
            return false;
        }
    }
}
