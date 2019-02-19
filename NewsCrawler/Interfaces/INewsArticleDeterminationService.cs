namespace NewsCrawler.Interfaces
{
    public interface INewsArticleDeterminationService
    {
        bool IsNewsArticle(string articleLink);
    }
}
