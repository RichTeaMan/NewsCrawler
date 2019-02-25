using NewsCrawler.Interfaces;
using System;

namespace NewsCrawler.Guardian
{
    public class GuardianArticleCleanerRunner : ArticleCleanerRunner
    {
        protected override string ArticleUrlContains { get; set; } = "theguardian.com";

        public GuardianArticleCleanerRunner(IServiceProvider serviceProvider, IArticleCleaner articleCleaner) : base(serviceProvider, articleCleaner)
        {
            CleanedArticleDirectory = "cleanedArticles/Guardian";
        }
    }
}
