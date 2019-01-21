using System;

namespace NewsCrawler
{
    public interface IArticlePublishedDateFetcherService
    {
        DateTimeOffset? FetchDate(string articleContent);
    }
}