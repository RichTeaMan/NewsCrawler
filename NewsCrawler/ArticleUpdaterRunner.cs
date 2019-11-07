using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class ArticleUpdaterRunner : IArticleUpdaterRunner
    {
        private readonly ILogger logger;

        private readonly INewsArticleTitleFetcherService newsArticleTitleFetcherService;

        private readonly IArticlePublishedDateFetcherService articlePublishedDateFetcherService;

        private readonly INewsArticleDeterminationService newsArticleDeterminationService;

        private readonly IServiceProvider serviceProvider;

        public ArticleUpdaterRunner(
            ILogger<ArticleUpdaterRunner> logger,
            INewsArticleTitleFetcherService newsArticleTitleFetcherService,
            IArticlePublishedDateFetcherService articlePublishedDateFetcherService,
            INewsArticleDeterminationService newsArticleDeterminationService,
            IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.newsArticleTitleFetcherService = newsArticleTitleFetcherService ?? throw new ArgumentNullException(nameof(newsArticleTitleFetcherService));
            this.articlePublishedDateFetcherService = articlePublishedDateFetcherService ?? throw new ArgumentNullException(nameof(articlePublishedDateFetcherService));
            this.newsArticleDeterminationService = newsArticleDeterminationService ?? throw new ArgumentNullException(nameof(newsArticleDeterminationService));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task RunTitleUpdater(string urlContains)
        {
            List<Article> articles = new List<Article>();

            logger.LogInformation("Updating titles of existing articles.");

            var articleBatcher = new ArticleBatcher(serviceProvider);
            int updates = 0;
            await articleBatcher.RunArticleBatch(
            a => a.Url.Contains(urlContains),
            article =>
            {
                bool hasUpdates = false;
                string title = Truncate(newsArticleTitleFetcherService.FetchTitle(article.ArticleContent.Content), Constants.MAX_TITLE_LENGTH);
                if (article.Title != title)
                {
                    article.Title = title;
                    hasUpdates = true;
                }

                bool isIndexPage = newsArticleDeterminationService.IsIndexPage(article.Url);
                if (isIndexPage != article.IsIndexPage)
                {
                    article.IsIndexPage = isIndexPage;
                    hasUpdates = true;
                }
                var publishedDate = articlePublishedDateFetcherService.FetchDate(article.ArticleContent.Content);
                if (publishedDate != article.PublishedDate)
                {
                    article.PublishedDate = publishedDate;
                    hasUpdates = true;
                }

                if (hasUpdates)
                {
                    updates++;
                }

                return Task.FromResult(hasUpdates);
            });

            logger.LogInformation($"{updates} articles updated.");
            logger.LogInformation("Title update complete!");
        }

        private string Truncate(string value, int maxLength)
        {
            if (value == null)
            {
                return null;
            }
            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}
