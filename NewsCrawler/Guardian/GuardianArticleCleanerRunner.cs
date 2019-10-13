using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using System;

namespace NewsCrawler.Guardian
{
    public class GuardianArticleCleanerRunner : ArticleCleanerRunner
    {
        protected override string ArticleUrlContains { get; set; } = "theguardian.com";

        public GuardianArticleCleanerRunner(ILogger<GuardianArticleCleanerRunner> logger, IServiceProvider serviceProvider, IArticleCleaner articleCleaner) : base(logger, serviceProvider, articleCleaner)
        {
            CleanedArticleDirectory = "cleanedArticles/Guardian";
        }
    }
}
