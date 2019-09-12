using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using System;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class NewsArticleFetcherRunner : INewsArticleFetcherRunner
    {

        private readonly IServiceProvider serviceProvider;

        private readonly int batchSize = 200;

        public NewsArticleFetcherRunner(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task RunFetcher()
        {
            Console.WriteLine("Starting migration.");

            int batchedArticles = 0;
            int totalArticles = 0;
            var postGresContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.Postgres.PostgresNewsArticleContext>();
            var sqlBatcher = new NewsCrawler.Persistence.ArticleBatcher(serviceProvider);
            await sqlBatcher.RunArticleBatch(
                sourceArticle => sourceArticle.RecordedDate < new DateTimeOffset(2019, 4, 1, 0, 0, 0, TimeSpan.Zero),
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
