using NewsCrawler.Persistence;

namespace NewsCrawler
{
    public interface IArticleCleaner
    {
        string CleanArticle(Article article);
    }
}
