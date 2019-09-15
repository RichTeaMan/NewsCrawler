using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NewsCrawler
{
    public class NewsArticleMigratorRunner : INewsArticleMigratorRunner
    {

        private readonly IServiceProvider serviceProvider;

        private readonly int batchSize = 100;

        public NewsArticleMigratorRunner(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task RunMigrator()
        {
            Console.WriteLine("Starting migration.");

            DateTimeOffset latestDate = DateTimeOffset.MinValue; ;
            using (var dataPostGresContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.Postgres.PostgresNewsArticleContext>())
            {
                if (dataPostGresContext.Articles.Any())
                {
                    latestDate = dataPostGresContext.Articles.Max(a => a.RecordedDate);
                }
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
                    if (batchedArticles >= batchSize)
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
            await FetchMissingArticles();

            Console.WriteLine("Migration complete!");
        }

        private async Task FetchMissingArticles()
        {
            Console.WriteLine("Finding missing articles...");
            string[] missingUrls = await FetchMissingUrls();

            Console.WriteLine($"{missingUrls.Length} missing articles found.");

            int batch = 0;
            var dataContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.NewsArticleContext>();
            var dataPostGresContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.Postgres.PostgresNewsArticleContext>();

            foreach (var url in missingUrls)
            {
                var sourceArticle = await dataContext.Articles.FirstAsync(a => a.Url == url);

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

                await dataPostGresContext.Articles.AddAsync(pgArticle);
                batch++;
                if (batch >= batchSize)
                {
                    batch = 0;
                    Console.WriteLine($"Saving...");

                    await dataPostGresContext.SaveChangesAsync();

                    dataContext.Dispose();
                    dataPostGresContext.Dispose();
                    dataContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.NewsArticleContext>();
                    dataPostGresContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.Postgres.PostgresNewsArticleContext>();

                }
            }
            await dataPostGresContext.SaveChangesAsync();

            dataContext.Dispose();
            dataPostGresContext.Dispose();

            Console.WriteLine("Missing articles saved.");
        }

        private async Task<string[]> FetchMissingUrls()
        {
            string[] missingUrls;
            using (var dataContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.NewsArticleContext>())
            using (var dataPostGresContext = serviceProvider.GetRequiredService<NewsCrawler.Persistence.Postgres.PostgresNewsArticleContext>())
            {
                var sqlUrlsArr = await dataContext.Articles.Select(a => a.Url).ToArrayAsync();
                var sqlUrls = new HashSet<string>(sqlUrlsArr);

                var postgresUrlsArr = await dataPostGresContext.Articles.Select(a => a.Url).ToArrayAsync();
                var postGresUrl = new HashSet<string>(postgresUrlsArr);

                missingUrls = sqlUrls.Where(u => !postGresUrl.Contains(u)).ToArray();
            }
            return missingUrls;
        }
    }
}
