using NewsCrawler.Persistence;

namespace NewsCrawler.Interfaces
{
    public interface IArticleCleaner
    {
        string CleanArticle(Article article);
    }
}
