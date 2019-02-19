using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System.Text.RegularExpressions;

namespace NewsCrawler.Bbc
{
    public class BbcIndexPageDeterminationService : IIndexPageDeterminationService
    {
        public bool IsIndexPage(Article article)
        {
            return !Regex.IsMatch(article.Url, @"\d") && !article.Url.Contains("correspondents");
        }
    }
}
