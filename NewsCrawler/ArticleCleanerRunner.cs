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
            var articleBatcher = new ArticleBatcher(serviceProvider);
            Directory.CreateDirectory(CleanedArticleDirectory);

            await articleBatcher.RunArticleBatch(
            article => article.Url.Contains(ArticleUrlContains),
            async article =>
            {
                string fileName = article?.Title;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    Console.WriteLine($"Article with ID {article.Id} does not have a title.");
                    fileName = article.Id.ToString();
                }
                foreach (var invalidFilenameChar in Path.GetInvalidFileNameChars())
                {
                    fileName = fileName.Replace(invalidFilenameChar.ToString(), string.Empty);
                }
                var clean = articleCleaner.CleanArticle(article);
                await File.WriteAllTextAsync(Path.Combine(CleanedArticleDirectory, $"{fileName}.txt"), clean);
                return false;
            });

            Console.WriteLine($"Completed cleaning articles containing '{ArticleUrlContains}'.");
        }
    }

}
