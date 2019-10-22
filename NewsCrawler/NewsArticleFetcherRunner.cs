using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsCrawler.Exceptions;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class NewsArticleFetcherRunner : INewsArticleFetcherRunner
    {
        private readonly ILogger logger;

        private readonly INewsArticleFetchService newsArticleFetchService;

        private readonly IServiceProvider serviceProvider;

        private readonly int batchSize = 50;

        public NewsArticleFetcherRunner(ILogger<NewsArticleFetcherRunner> logger, INewsArticleFetchService newsArticleFetchService, IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.newsArticleFetchService = newsArticleFetchService ?? throw new ArgumentNullException(nameof(newsArticleFetchService));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<FetcherResult> RunFetcher()
        {
            string newsSource;
            logger.LogInformation("Loading existing articles.");
            HashSet<string> existingArticles = null;
            int existingArticlesAttempt = 0;
            while (existingArticles == null)
            {
                try
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedNewsArticleContext = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>();
                        var newsArticleFinderService = scope.ServiceProvider.GetRequiredService<INewsArticleFinderService>();
                        var source = await newsArticleFinderService.FetchSource(scopedNewsArticleContext);
                        var urls = await scopedNewsArticleContext.Articles.Where(a => a.Source == source).Select(a => a.Url).ToArrayAsync();
                        existingArticles = new HashSet<string>(urls);
                        logger.LogInformation($"{existingArticles.Count} existing articles from source {source.Name} loaded.");
                    }
                    existingArticlesAttempt++;
                }
                catch (Exception)
                {
                    if (existingArticlesAttempt >= 5)
                    {
                        throw;
                    }
                }
            }

            List<string> articleLinks;
            using (var scope = serviceProvider.CreateScope())
            {
                var newsArticleFinderService = scope.ServiceProvider.GetRequiredService<INewsArticleFinderService>();
                articleLinks = newsArticleFinderService.FindNewsArticles().Distinct().Where(a => !existingArticles.Contains(a)).ToList();
                logger.LogInformation($"Getting articles from news source: '{newsArticleFinderService.SourceName}'");
                newsSource = newsArticleFinderService.SourceName;
            }

            logger.LogInformation($"Found {articleLinks.Count()} articles.");

            var articles = new List<Article>();
            int fetchedArticleCount = 0;
            int errorCount = 0;
            foreach (var articleLink in articleLinks)
            {
                try
                {
                    var article = await newsArticleFetchService.FetchArticleAsync(articleLink);
                    articles.Add(article);
                    fetchedArticleCount++;
                    if (fetchedArticleCount % 10 == 0)
                    {
                        logger.LogInformation($"{fetchedArticleCount} of {articleLinks.Count()} articles loaded.");
                    }

                    if (articles.Count >= batchSize)
                    {
                        await SaveArticles(articles);
                        articles = new List<Article>();
                    }
                }
                catch (DbUpdateException)
                {
                    throw;
                }
                catch (UrlTooLongException ex)
                {
                    logger.LogError(ex, "URL too long exception.");
                    errorCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred retrieving '{articleLink}'.");
                    errorCount++;
                }
            }

            if (articles.Any())
            {
                await SaveArticles(articles);
            }

            logger.LogInformation($"Complete: {fetchedArticleCount} articles loaded.");
            logger.LogInformation("Crawling complete!");

            return new FetcherResult(newsSource, articleLinks.Count, fetchedArticleCount, errorCount);
        }

        private async Task SaveArticles(List<Article> articles)
        {
            using (var scope = serviceProvider.CreateScope())
            using (var scopedPostgresNewsArticleContext = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>())
            {
                logger.LogInformation("Saving articles...");
                var newsArticleFinderService = scope.ServiceProvider.GetRequiredService<INewsArticleFinderService>();
                var source = await newsArticleFinderService.FetchSource(scopedPostgresNewsArticleContext);

                articles.ForEach(a => a.Source = source);

                await scopedPostgresNewsArticleContext.Articles.AddRangeAsync(articles);
                await scopedPostgresNewsArticleContext.SaveChangesAsync();
            }
        }
    }
}
