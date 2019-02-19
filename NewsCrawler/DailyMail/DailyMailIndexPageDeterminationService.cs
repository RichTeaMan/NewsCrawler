using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System.Text.RegularExpressions;

namespace NewsCrawler.DailyMail
{
    public class DailyMailIndexPageDeterminationService : IIndexPageDeterminationService
    {
        public bool IsIndexPage(Article article)
        {
            return !Regex.IsMatch(article.Url, @"\d") && !article.Url.Contains("news/article-");
        }
    }
}
