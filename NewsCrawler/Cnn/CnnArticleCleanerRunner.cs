using NewsCrawler.Interfaces;
using System;

namespace NewsCrawler.Cnn
{
    public class CnnArticleCleanerRunner : ArticleCleanerRunner
    {
        protected override string ArticleUrlContains { get; set; } = "edition.cnn.com";

        public CnnArticleCleanerRunner(IServiceProvider serviceProvider, IArticleCleaner articleCleaner) : base(serviceProvider, articleCleaner)
        {
            CleanedArticleDirectory = "cleanedArticles/CNN";
        }
    }
}
