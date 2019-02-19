namespace NewsCrawler.Interfaces
{
    public interface INewsArticleTitleFetcherService
    {
        string FetchTitle(string articleContent);
    }
}
