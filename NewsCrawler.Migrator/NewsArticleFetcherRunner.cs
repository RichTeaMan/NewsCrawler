using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace NewsCrawler
{
    public class NewsArticleFetcherRunner : INewsArticleFetcherRunner
    {

        private readonly IServiceProvider serviceProvider;

        private readonly int batchSize = 100;

        public NewsArticleFetcherRunner(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task RunFetcher()
        {
            Console.WriteLine("Starting migration.");

            DateTimeOffset latestDate;
            using (var dataPostGresContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.Postgres.PostgresNewsArticleContext>())
            {
                latestDate = dataPostGresContext.Articles.Max(a => a.RecordedDate);
            }

            Console.WriteLine($"Getting articles recorded after {latestDate}");

            int batchedArticles = 0;
            int totalArticles = 0;
            var postGresContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.Postgres.PostgresNewsArticleContext>();
            var sqlBatcher = new NewsCrawler.Persistence.ArticleBatcher(serviceProvider);
            await sqlBatcher.RunArticleBatch(
                sourceArticle => sourceArticle.RecordedDate > latestDate,
                async sourceArticle =>
            {
                NewsCrawler.Persistence.Postgres.Article pgArticle = new Persistence.Postgres.Article()
                {
                    CleanedContent = sourceArticle.CleanedContent,
                    CleanedContentLength = sourceArticle.CleanedContentLength,
                    Content = sourceArticle.Content,
                    ContentLength = sourceArticle.ContentLength,
                    IsIndexPage = sourceArticle.IsIndexPage,
                    NewsSource = sourceArticle.NewsSource,
                    PublishedDate = sourceArticle.PublishedDate,
                    RecordedDate = sourceArticle.RecordedDate,
                    Title = sourceArticle.Title,
                    Url = sourceArticle.Url
                };
                await postGresContext.AddAsync(pgArticle);
                batchedArticles++;
                totalArticles++;
                if (batchedArticles > batchSize)
                {
                    batchedArticles = 0;
                    Console.WriteLine($"Saving...");
                    await postGresContext.SaveChangesAsync();
                    postGresContext.Dispose();
                    postGresContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.Postgres.PostgresNewsArticleContext>();
                    Console.WriteLine($"{totalArticles} articles migrated.");
                }

                return false;
            });

            await postGresContext.SaveChangesAsync();
            postGresContext.Dispose();
            Console.WriteLine($"{totalArticles} articles migrated.");

            Console.WriteLine("Migration complete!");
        }
    }
}
