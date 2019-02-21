using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
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

        private readonly int batchSize = 100;

        public NewsArticleFetcherRunner(INewsArticleFetchService newsArticleFetchService, IServiceProvider serviceProvider)
        {
            this.newsArticleFetchService = newsArticleFetchService ?? throw new ArgumentNullException(nameof(newsArticleFetchService));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task RunFetcher()
        {
            try
            {

                Console.WriteLine("Loading existing articles.");
                HashSet<string> existingArticles;
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedNewsArticleContext = scope.ServiceProvider.GetRequiredService<NewsArticleContext>();
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

                List<Article> articles = new List<Article>();
                int totalArticles = 0;
                foreach (var articleLink in articleLinks)
                {
                    try
                    {
                        var article = await newsArticleFetchService.FetchArticleAsync(articleLink);
                        article.NewsSource = sourceName;
                        articles.Add(article);
                        totalArticles++;
                        if (totalArticles % 10 == 0)
                        {
                            Console.WriteLine($"{articles.Count()} of {articleLinks.Count()} articles loaded.");
                        }

                        if (articles.Count >= batchSize)
                        {
                            using (var scope = serviceProvider.CreateScope())
                            {
                                Console.WriteLine("Saving articles...");
                                var scopedNewsArticleContext = scope.ServiceProvider.GetRequiredService<NewsArticleContext>();
                                await scopedNewsArticleContext.Articles.AddRangeAsync(articles);
                                await scopedNewsArticleContext.SaveChangesAsync();
                                articles = new List<Article>();
                            }
                        }
                    }
                    catch (DbUpdateException)
                    {
                        throw;
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
                    using (var scope = serviceProvider.CreateScope())
                    {
                        Console.WriteLine("Saving articles...");
                        var scopedNewsArticleContext = scope.ServiceProvider.GetRequiredService<NewsArticleContext>();
                        await scopedNewsArticleContext.Articles.AddRangeAsync(articles);
                        await scopedNewsArticleContext.SaveChangesAsync();
                    }
                }

                Console.WriteLine($"Complete: {totalArticles} articles loaded.");

                Console.WriteLine("Crawling complete!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred fetching: {ex.Message}");
                Exception inner = ex.InnerException;
                while (inner != null)
                {
                    Console.WriteLine($"Inner exception: {inner.Message}");
                    inner = inner.InnerException;
                }
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
