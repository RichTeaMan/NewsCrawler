using NewsCrawler.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class NewsArticleFetcherRunner : INewsArticleFetcherRunner
    {
        private readonly INewsArticleFinderService newsArticleFinderService;

        private readonly INewsArticleFetchService newsArticleFetchService;

        private readonly NewsArticleContext newsArticleContext;

        public NewsArticleFetcherRunner(INewsArticleFinderService newsArticleFinderService, INewsArticleFetchService newsArticleFetchService, NewsArticleContext newsArticleContext)
        {
            this.newsArticleFinderService = newsArticleFinderService ?? throw new ArgumentNullException(nameof(newsArticleFinderService));
            this.newsArticleFetchService = newsArticleFetchService ?? throw new ArgumentNullException(nameof(newsArticleFetchService));
            this.newsArticleContext = newsArticleContext ?? throw new ArgumentNullException(nameof(newsArticleContext));
        }

        public async Task RunFetcher()
        {
            List<Article> articles = new List<Article>();

            Console.WriteLine("Loading existing articles.");
            var existingArticles = new HashSet<string>(newsArticleContext.Articles.Select(a => a.Url));
            Console.WriteLine($"{existingArticles.Count} existing articles loaded.");

            Console.WriteLine("Getting articles.");

            var articleLinks = newsArticleFinderService.FindNewsArticles().Distinct().Where(a => !existingArticles.Contains(a)).ToList();

            Console.WriteLine($"Found {articleLinks.Count()} articles.");

            foreach (var articleLink in articleLinks)
            {
                try
                {
                    var article = await newsArticleFetchService.FetchArticleAsync(articleLink);
                    articles.Add(article);
                    if (articles.Count() % 10 == 0)
                    {
                        Console.WriteLine($"{articles.Count()} articles loaded.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            Console.WriteLine("Articles loaded.");

            Console.WriteLine("Saving articles...");

            await newsArticleContext.Articles.AddRangeAsync(articles);

            await newsArticleContext.SaveChangesAsync();

            Console.WriteLine("Complete!");
        }
    }
}
