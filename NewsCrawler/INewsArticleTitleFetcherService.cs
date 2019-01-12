namespace NewsCrawler
{
    public interface INewsArticleTitleFetcherService
    {
        string FetchTitle(string articleContent);
    }
}