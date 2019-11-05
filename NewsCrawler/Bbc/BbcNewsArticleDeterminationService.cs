using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System.Text.RegularExpressions;

namespace NewsCrawler.Bbc
{
    public class BbcNewsArticleDeterminationService : INewsArticleDeterminationService
    {
        public bool IsNewsArticle(string articleLink)
        {
            return articleLink?.StartsWith("https://www.bbc.co.uk/news/") == true;
        }

        public bool IsIndexPage(string articleLink)
        {
            return !Regex.IsMatch(articleLink, @"\d") && !articleLink.Contains("correspondents");
        }
    }
}
