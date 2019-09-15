using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        private readonly INewsArticleFetchService newsArticleFetchService;

        private readonly IServiceProvider serviceProvider;

        private readonly int batchSize = 50;

        public NewsArticleFetcherRunner(INewsArticleFetchService newsArticleFetchService, IServiceProvider serviceProvider)
        {
            this.newsArticleFetchService = newsArticleFetchService ?? throw new ArgumentNullException(nameof(newsArticleFetchService));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task RunFetcher()
        {
            Console.WriteLine("Loading existing articles.");
            HashSet<string> existingArticles;
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedNewsArticleContext = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>();
                existingArticles = new HashSet<string>(scopedNewsArticleContext.Articles.Select(a => a.Url));
            }
            Console.WriteLine($"{existingArticles.Count} existing articles loaded.");

            List<string> articleLinks;
            string sourceName;
            using (var scope = serviceProvider.CreateScope())
            {
                var newsArticleFinderService = scope.ServiceProvider.GetRequiredService<INewsArticleFinderService>();
                articleLinks = newsArticleFinderService.FindNewsArticles().Distinct().Where(a => !existingArticles.Contains(a)).ToList();
                sourceName = newsArticleFinderService.SourceName;
            }
            Console.WriteLine($"Getting articles from news source: '{sourceName}'");

            Console.WriteLine($"Found {articleLinks.Count()} articles.");

            var articles = new List<Article>();
            int fetchedArticles = 0;
            foreach (var articleLink in articleLinks)
            {
                try
                {
                    var article = await newsArticleFetchService.FetchArticleAsync(articleLink);
                    article.NewsSource = sourceName;
                    articles.Add(article);
                    fetchedArticles++;
                    if (fetchedArticles % 10 == 0)
                    {
                        Console.WriteLine($"{fetchedArticles} of {articleLinks.Count()} articles loaded.");
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
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred retrieving '{articleLink}': {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
            }

            if (articles.Any())
            {
                Console.WriteLine("Saving articles...");
                await SaveArticles(articles);
            }

            Console.WriteLine($"Complete: {fetchedArticles} articles loaded.");
            Console.WriteLine("Crawling complete!");
        }

        private async Task SaveArticles(List<Article> articles)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                Console.WriteLine("Saving articles to Postgres...");
                using (var scopedPostgresNewsArticleContext = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>())
                {
                    await scopedPostgresNewsArticleContext.Articles.AddRangeAsync(articles);
                    await scopedPostgresNewsArticleContext.SaveChangesAsync();
                }
            }
        }
    }
}
