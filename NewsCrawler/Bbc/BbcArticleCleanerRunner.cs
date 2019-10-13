using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using System;

namespace NewsCrawler.Bbc
{
    public class BbcArticleCleanerRunner : ArticleCleanerRunner
    {
        protected override string ArticleUrlContains { get; set; } = "bbc.co.uk";

        public BbcArticleCleanerRunner(ILogger<BbcArticleCleanerRunner> logger, IServiceProvider serviceProvider, IArticleCleaner articleCleaner) : base(logger, serviceProvider, articleCleaner)
        {
            CleanedArticleDirectory = "cleanedArticles/BBC";
        }
    }
}
