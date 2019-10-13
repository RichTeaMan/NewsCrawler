using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using System;

namespace NewsCrawler.DailyMail
{
    public class DailyMailArticleCleanerRunner : ArticleCleanerRunner
    {
        protected override string ArticleUrlContains { get; set; } = "dailymail.co.uk";

        public DailyMailArticleCleanerRunner(ILogger<DailyMailArticleCleanerRunner> logger, IServiceProvider serviceProvider, IArticleCleaner articleCleaner) : base(logger, serviceProvider, articleCleaner)
        {
            CleanedArticleDirectory = "cleanedArticles/DailyMail";
        }
    }
}
