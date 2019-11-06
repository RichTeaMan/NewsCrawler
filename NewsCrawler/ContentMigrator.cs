using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class ContentMigrator
    {
        private readonly ILogger logger;

        private readonly IServiceProvider serviceProvider;

        public ContentMigrator(ILogger<ContentMigrator> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task RunMigrator()
        {
            logger.LogInformation("Running article migrator.");
            ArticleBatcher articleBatcher = new ArticleBatcher(serviceProvider);
            articleBatcher.SplitArticleCount = 5;
            await articleBatcher.RunArticleBatch(a => !a.IsTransferred,
                a =>
                {
                    var content = new ArticleContent()
                    {
                        Content = a.Content
                    };
                    a.ArticleContent = content;

                    if (a.CleanedContent != null)
                    {
                        var cleaned = new ArticleCleanedContent()
                        {
                            CleanedContent = a.CleanedContent
                        };
                        a.ArticleCleanedContent = cleaned;
                        a.CleanedContent = string.Empty;
                    }
                    a.Content = string.Empty;
                    a.IsTransferred = true;

                    return Task.FromResult(true);
                });

            logger.LogInformation("Article migrator complete.");
            logger.LogInformation("Article crawler closing.");
        }
    }
}
