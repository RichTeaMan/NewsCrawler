using NewsCrawler.Interfaces;
using System;

namespace NewsCrawler.DailyMail
{
    public class DailyMailArticleCleanerRunner : ArticleCleanerRunner
    {
        protected override string ArticleUrlContains { get; set; } = "dailymail.co.uk";

        public DailyMailArticleCleanerRunner(IServiceProvider serviceProvider, IArticleCleaner articleCleaner) : base(serviceProvider, articleCleaner)
        {
            CleanedArticleDirectory = "cleanedArticles/DailyMail";
        }
    }
}
