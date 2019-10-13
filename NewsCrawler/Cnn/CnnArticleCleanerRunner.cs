using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using System;

namespace NewsCrawler.Cnn
{
    public class CnnArticleCleanerRunner : ArticleCleanerRunner
    {
        protected override string ArticleUrlContains { get; set; } = "edition.cnn.com";

        public CnnArticleCleanerRunner(ILogger<CnnArticleCleanerRunner> logger, IServiceProvider serviceProvider, IArticleCleaner articleCleaner) : base(logger, serviceProvider, articleCleaner)
        {
            CleanedArticleDirectory = "cleanedArticles/CNN";
        }
    }
}
