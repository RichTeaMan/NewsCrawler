using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class ArticleUpdaterRunner : IArticleUpdaterRunner
    {
        private readonly INewsArticleTitleFetcherService newsArticleTitleFetcherService;

        private readonly IArticlePublishedDateFetcherService articlePublishedDateFetcherService;

        private readonly INewsArticleDeterminationService newsArticleDeterminationService;

        private readonly IServiceProvider serviceProvider;

        public ArticleUpdaterRunner(INewsArticleTitleFetcherService newsArticleTitleFetcherService,
            INewsArticleDeterminationService newsArticleDeterminationService,
            IArticlePublishedDateFetcherService articlePublishedDateFetcherService,
            IServiceProvider serviceProvider)
        {
            this.newsArticleTitleFetcherService = newsArticleTitleFetcherService;
            this.newsArticleDeterminationService = newsArticleDeterminationService;
            this.articlePublishedDateFetcherService = articlePublishedDateFetcherService;
            this.serviceProvider = serviceProvider;
        }

        public async Task RunTitleUpdater(string urlContains)
        {
            List<Article> articles = new List<Article>();

            Console.WriteLine("Updating titles of existing articles.");

            var articleBatcher = new ArticleBatcher(serviceProvider);
            int updates = 0;
            await articleBatcher.RunArticleBatch(
            a => a.Url.Contains(urlContains),
            article =>
            {
                bool hasUpdates = false;
                string title = newsArticleTitleFetcherService.FetchTitle(article.Content);
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
                var publishedDate = articlePublishedDateFetcherService.FetchDate(article.Content);
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

            Console.WriteLine($"{updates} articles updated.");
            Console.WriteLine("Title update complete!");
        }
    }
}
