using NewsCrawler.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public abstract class ArticleCleanerRunner : IArticleCleanerRunner
    {
        private readonly IServiceProvider serviceProvider;

        private readonly IArticleCleaner articleCleaner;

        protected abstract string ArticleUrlContains { get; set; }

        protected string CleanedArticleDirectory { get; set; } = "cleanedArticles";

        public ArticleCleanerRunner(IServiceProvider serviceProvider, IArticleCleaner articleCleaner)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.articleCleaner = articleCleaner ?? throw new ArgumentNullException(nameof(articleCleaner));
        }

        public async Task CleanArticles()
        {
            Console.WriteLine($"Cleaning articles containing '{ArticleUrlContains}'.");
            var articleBatcher = new Persistence.ArticleBatcher(serviceProvider);
            Directory.CreateDirectory(CleanedArticleDirectory);

            await articleBatcher.RunArticleBatch(
            article => article.Url.Contains(ArticleUrlContains),
            async article =>
            {
                var clean = articleCleaner.CleanArticle(article.Content);
                if (clean != article.CleanedContent)
                {
                    article.CleanedContent = clean;
                    int cleanLength = 0;
                    if (!string.IsNullOrEmpty(clean))
                    {
                        cleanLength = clean.Length;
                    }
                    article.CleanedContentLength = cleanLength;
                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            });

            Console.WriteLine($"Completed cleaning articles containing '{ArticleUrlContains}'.");
        }
    }

}
