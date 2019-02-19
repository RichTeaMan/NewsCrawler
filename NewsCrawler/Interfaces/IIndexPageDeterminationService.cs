using NewsCrawler.Persistence;

namespace NewsCrawler.Interfaces
{
    public interface IIndexPageDeterminationService
    {
        bool IsIndexPage(Article article);
    }
}
