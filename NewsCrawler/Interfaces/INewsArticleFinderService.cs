using NewsCrawler.Persistence.Postgres;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsCrawler.Interfaces
{
    public interface INewsArticleFinderService
    {
        /// <summary>
        /// Fetches the news source this will find articles from.
        /// </summary>
        Task<Source> FetchSource(PostgresNewsArticleContext postgresNewsArticleContext);

        string SourceName { get; }

        IEnumerable<string> FindNewsArticles();
    }
}
