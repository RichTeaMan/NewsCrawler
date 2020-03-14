using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
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

        public async Task RunCrawler(string databaseString)
        {
            logger.LogInformation("Running article crawler.");
            var fetcherResults = new List<FetcherResult>();
            foreach (var serviceProvider in ServiceProviderFactory.CreateServiceProviders(databaseString))
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var newsArticleFetcherRunner = scope.ServiceProvider.GetRequiredService<INewsArticleFetcherRunner>();
                    var result = await newsArticleFetcherRunner.RunFetcher();
                    fetcherResults.Add(result);
                }
            }
            logger.LogInformation("Article crawler complete.");
            fetcherResults.ForEach(r => LogFetcherResult(r));
            LogFetcherResult(fetcherResults.Aggregate((x, y) => x + y));
            logger.LogInformation("Article crawler closing.");
        }

        private void LogFetcherResult(FetcherResult fetcherResult)
        {
            logger.LogInformation(
                $"           News Source: {fetcherResult.NewsSource}\n" +
                $"           Links Found: {fetcherResult.ArticleLinksFound}\n" +
                $"        Articles Saved: {fetcherResult.ArticlesSaved}\n" +
                $"           Error Count: {fetcherResult.ErrorCounts}\n" +
                $"Elapsed Time (minutes): {fetcherResult.ElapsedTimeSpan.TotalMinutes.ToString("0.#")}\n");
        }
    }
}
