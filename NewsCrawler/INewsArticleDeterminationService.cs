namespace NewsCrawler
{
    public interface INewsArticleDeterminationService
    {
        bool IsNewsArticle(string articleLink);
    }
}