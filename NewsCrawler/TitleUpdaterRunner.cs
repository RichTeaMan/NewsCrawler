﻿using NewsCrawler.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class TitleUpdaterRunner : ITitleUpdaterRunner
    {
        private readonly INewsArticleTitleFetcherService newsArticleTitleFetcherService;

        private readonly IIndexPageDeterminationService indexPageDeterminationService;

        private readonly NewsArticleContext newsArticleContext;



        public TitleUpdaterRunner(INewsArticleTitleFetcherService newsArticleTitleFetcherService, IIndexPageDeterminationService indexPageDeterminationService, NewsArticleContext newsArticleContext)
        {
            this.newsArticleTitleFetcherService = newsArticleTitleFetcherService;
            this.indexPageDeterminationService = indexPageDeterminationService;
            this.newsArticleContext = newsArticleContext;
        }

        public async Task RunTitleUpdater()
        {
            List<Article> articles = new List<Article>();

            Console.WriteLine("Updating titles of existing articles.");
            int articleCount = newsArticleContext.Articles.Count();
            Console.WriteLine($"{articleCount} articles to update.");

            int updates = 0;
            int articlesChecked = 0;
            foreach(var article in newsArticleContext.Articles)
            {
                bool hasUpdates = false;
                string title = newsArticleTitleFetcherService.FetchTitle(article.Content);
                if (article.Title != title)
                {
                    article.Title = title;
                    hasUpdates = true;
                }

                bool isIndexPage = indexPageDeterminationService.IsIndexPage(article);
                if (isIndexPage != article.IsIndexPage)
                {
                    article.IsIndexPage = isIndexPage;
                    hasUpdates = true;
                }

                if (hasUpdates)
                {
                    updates++;
                }

                articlesChecked++;
                if (articlesChecked % 20 == 0)
                {
                    Console.WriteLine($"{articlesChecked} of {articleCount} articles checked. {updates} updates pending.");
                }
            }
            Console.WriteLine($"{articlesChecked} of {articleCount} articles checked. {updates} updates pending.");

            Console.WriteLine("Saving articles...");

            await newsArticleContext.SaveChangesAsync();

            Console.WriteLine("Title update complete!");
        }
    }
}