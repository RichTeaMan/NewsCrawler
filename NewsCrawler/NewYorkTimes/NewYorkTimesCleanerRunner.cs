using NewsCrawler.Interfaces;
using System;

namespace NewsCrawler.NewYorkTimes
{
    public class NewYorkTimesArticleCleanerRunner : ArticleCleanerRunner
    {
        protected override string ArticleUrlContains { get; set; } = "www.nytimes.com/";

        public NewYorkTimesArticleCleanerRunner(IServiceProvider serviceProvider, IArticleCleaner articleCleaner) : base(serviceProvider, articleCleaner)
        {
            CleanedArticleDirectory = "cleanedArticles/NewYorkTimes";
        }
    }
}
