using System.Collections.Generic;

namespace NewsCrawler.Interfaces
{
    public interface INewsArticleFinderService
    {
        /// <summary>
        /// Gets a human readable name of the news source this will find articles from.
        /// </summary>
        string SourceName { get; }

        IEnumerable<string> FindNewsArticles();
    }
}
