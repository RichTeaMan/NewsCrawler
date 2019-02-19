using System;

namespace NewsCrawler.Interfaces
{
    public interface IArticlePublishedDateFetcherService
    {
        DateTimeOffset? FetchDate(string articleContent);
    }
}
