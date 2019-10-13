using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using System;

namespace NewsCrawler.NewYorkTimes
{
    public class NewYorkTimesArticleCleanerRunner : ArticleCleanerRunner
    {
        protected override string ArticleUrlContains { get; set; } = "www.nytimes.com/";

        public NewYorkTimesArticleCleanerRunner(ILogger<NewYorkTimesArticleCleaner> logger, IServiceProvider serviceProvider, IArticleCleaner articleCleaner) : base(logger, serviceProvider, articleCleaner)
        {
            CleanedArticleDirectory = "cleanedArticles/NewYorkTimes";
        }
    }
}
