using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public abstract class ArticleCleanerRunner : IArticleCleanerRunner
    {
        private readonly ILogger logger;

        private readonly IServiceProvider serviceProvider;

        private readonly IArticleCleaner articleCleaner;

        protected abstract string ArticleUrlContains { get; set; }

        protected string CleanedArticleDirectory { get; set; } = "cleanedArticles";

        protected ArticleCleanerRunner(ILogger logger, IServiceProvider serviceProvider, IArticleCleaner articleCleaner)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.articleCleaner = articleCleaner ?? throw new ArgumentNullException(nameof(articleCleaner));
        }

        public async Task CleanArticles()
        {
            logger.LogInformation($"Cleaning articles containing '{ArticleUrlContains}'.");
            var articleBatcher = new ArticleBatcher(serviceProvider)
            {
                IncludeContent = true
            };
            Directory.CreateDirectory(CleanedArticleDirectory);

            await articleBatcher.RunArticleBatch(
            article => article.Url.Contains(ArticleUrlContains),
            async article =>
            {
                bool updateRequired = await article.ArticleContent.UpdateCompression();
                var clean = articleCleaner.CleanArticle(article.ArticleContent.Content);
                if (clean != article.ArticleCleanedContent?.CleanedContent)
                {
                    if (article.ArticleCleanedContent == null)
                    {
                        article.ArticleCleanedContent = new ArticleCleanedContent();
                    }
                    article.ArticleCleanedContent.CleanedContent = clean;
                    int cleanLength = 0;
                    if (!string.IsNullOrEmpty(clean))
                    {
                        cleanLength = clean.Length;
                    }
                    article.CleanedContentLength = cleanLength;
                    updateRequired = true;
                }
                return updateRequired;
            });

            logger.LogInformation($"Completed cleaning articles containing '{ArticleUrlContains}'.");
        }
    }

}
