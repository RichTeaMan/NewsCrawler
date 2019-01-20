using NewsCrawler.Persistence;

namespace NewsCrawler
{
    public interface IIndexPageDeterminationService
    {
        bool IsIndexPage(Article article);
    }
}