using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsCrawler.Exceptions;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class NewsArticleFetcherRunner : INewsArticleFetcherRunner
    {
        private readonly ILogger logger;

        private readonly INewsArticleFetchService newsArticleFetchService;

        private readonly IServiceProvider serviceProvider;

        private readonly int batchSize = 50;

        private readonly int maximumArticleRetrievalAttempt = 5;

        /// <summary>
        /// Batch cut off for how many bytes should be insterted in a single batch.
        /// </summary>
        private readonly int byteBatchCount = 5 * 1000 * 1000;

        public NewsArticleFetcherRunner(ILogger<NewsArticleFetcherRunner> logger, INewsArticleFetchService newsArticleFetchService, IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.newsArticleFetchService = newsArticleFetchService ?? throw new ArgumentNullException(nameof(newsArticleFetchService));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<FetcherResult> RunFetcher()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string newsSource;
            logger.LogInformation("Loading existing articles.");
            HashSet<string> existingArticles = null;
            int existingArticlesAttempt = 0;
            while (existingArticles == null)
            {
                try
                {
                    existingArticlesAttempt++;
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedNewsArticleContext = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>();
                        var newsArticleFinderService = scope.ServiceProvider.GetRequiredService<INewsArticleFinderService>();
                        var source = await newsArticleFinderService.FetchSource(scopedNewsArticleContext);
                        var urls = await scopedNewsArticleContext.Articles.Where(a => a.Source == source).Select(a => a.Url).ToArrayAsync();
                        existingArticles = new HashSet<string>(urls);
                        logger.LogInformation($"{existingArticles.Count} existing articles from source {source.Name} loaded.");
                    }
                }
                catch (TimeoutException)
                {
                    if (existingArticlesAttempt >= maximumArticleRetrievalAttempt)
                    {
                        logger.LogError($"Failed to load existing articles after {maximumArticleRetrievalAttempt} attempts.");
                        throw;
                    }
                    else
                    {
                        logger.LogWarning($"Loading existing articles attempt {existingArticlesAttempt} of {maximumArticleRetrievalAttempt} failed due to a timeout error. Trying again...");
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
                catch (HttpClientException ex)
                {
                    logger.LogError($"Article returned non success code: {ex.StatusCode} - '{articleLink}'.");
                    errorCount++;
                }
            }

            if (articles.Any())
            {
                await SaveArticles(articles);
            }

            logger.LogInformation($"Complete: {fetchedArticleCount} articles loaded.");
            logger.LogInformation("Crawling complete!");
            stopwatch.Stop();

            return new FetcherResult(newsSource, articleLinks.Count, fetchedArticleCount, errorCount, stopwatch.Elapsed);
        }

        private async Task SaveArticles(List<Article> articles)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var scope = serviceProvider.CreateScope())
            using (var scopedPostgresNewsArticleContext = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>())
            {
                logger.LogInformation("Saving articles...");
                logger.LogInformation("   Loading source...");
                var newsArticleFinderService = scope.ServiceProvider.GetRequiredService<INewsArticleFinderService>();
                var source = await newsArticleFinderService.FetchSource(scopedPostgresNewsArticleContext);
                logger.LogInformation($"   Source loaded in {stopwatch.Elapsed.TotalSeconds} seconds.");

                articles.ForEach(a => a.Source = source);
                var articleStack = new Stack<Article>(articles);
                var batchedArticles = new List<Article>();
                int batchBytes = 0;

                while (articleStack.Any())
                {
                    var poppedArticle = articleStack.Pop();
                    batchBytes = batchBytes + poppedArticle.CleanedContentLength + poppedArticle.ContentLength;
                    batchedArticles.Add(poppedArticle);

                    if (batchBytes > byteBatchCount || !articleStack.Any())
                    {
                        await scopedPostgresNewsArticleContext.Articles.AddRangeAsync(batchedArticles);
                        logger.LogInformation($"   Submitting {batchedArticles.Count} articles of combined size {batchBytes}...");
                        await scopedPostgresNewsArticleContext.SaveChangesAsync();
                        logger.LogInformation($"   Batch articles saved in {stopwatch.Elapsed.TotalSeconds} seconds.");

                        batchedArticles = new List<Article>();
                        batchBytes = 0;
                    }
                }
            }
            logger.LogInformation($"   Articles saved in {stopwatch.Elapsed.TotalSeconds} seconds.");
            stopwatch.Stop();
        }
    }
}
