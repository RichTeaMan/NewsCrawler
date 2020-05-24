using NewsCrawler.Interfaces;
using System;
using System.Text.RegularExpressions;

namespace NewsCrawler.Cnn
{
    public class CnnArticleDeterminationService : INewsArticleDeterminationService
    {
        private readonly Regex newsRegex = new Regex(@"https://edition.cnn.com/\d{4}/\d{2}/\d{2}/");

        public bool IsNewsArticle(string articleLink)
        {
            bool isNewsArticle = false;
            if (!string.IsNullOrEmpty(articleLink))
            {
                isNewsArticle = articleLink.EndsWith("/index.html") && newsRegex.IsMatch(articleLink);
            }
            return isNewsArticle;
        }

        public bool IsIndexPage(string articleLink)
        {
            return articleLink?.StartsWith("https://edition.cnn.com/") == true && !IsNewsArticle(articleLink);
        }
    }
}
