using NewsCrawler.Interfaces;
using System;

namespace NewsCrawler.Bbc
{
    public class BbcArticleCleanerRunner : ArticleCleanerRunner
    {
        protected override string ArticleUrlContains { get; set; } = "bbc.co.uk";

        public BbcArticleCleanerRunner(IServiceProvider serviceProvider, IArticleCleaner articleCleaner) : base(serviceProvider, articleCleaner)
        {
            CleanedArticleDirectory = "cleanedArticles/BBC";
        }
    }
}
