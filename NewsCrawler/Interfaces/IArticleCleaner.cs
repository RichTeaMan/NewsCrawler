using NewsCrawler.Persistence;

namespace NewsCrawler.Interfaces
{
    public interface IArticleCleaner
    {
        string CleanArticle(string articleContent);
    }
}
