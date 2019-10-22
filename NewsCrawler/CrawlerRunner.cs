using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class CrawlerRunner
    {
        private readonly ILogger logger;

        public CrawlerRunner(ILogger<CrawlerRunner> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunCrawler()
        {
            logger.LogInformation("Running article crawler.");
            var fetcherResults = new List<FetcherResult>();
            foreach (var serviceProvider in ServiceProviderFactory.CreateServiceProviders())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var newsArticleFetcherRunner = scope.ServiceProvider.GetRequiredService<INewsArticleFetcherRunner>();
                    var result = await newsArticleFetcherRunner.RunFetcher();
                    LogFetcherResult(result);
                    fetcherResults.Add(result);
                }
            }
            logger.LogInformation("Article crawler complete.");
            fetcherResults.ForEach(r => LogFetcherResult(r));
            LogFetcherResult(fetcherResults.Aggregate((x, y) => x + y));
        }

        private void LogFetcherResult(FetcherResult fetcherResult)
        {
            logger.LogInformation($"\tNews Source: {fetcherResult.NewsSource}" +
                $"\n\tLinks Found: {fetcherResult.ArticleLinksFound}" +
                $"\n\tArticles Saved: {fetcherResult.ArticlesSaved}" +
                $"\n\tError Count: {fetcherResult.ErrorCounts}" +
                $"\n\tElapsed Time (minutes): {fetcherResult.ElapsedTimeSpan.TotalMinutes.ToString("0.#")}\n");
        }
    }
}
