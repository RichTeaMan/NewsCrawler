using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class ArticleBatcher
    {
        private const int SplitArticleCount = 200;

        private readonly IServiceProvider serviceProvider;

        public ArticleBatcher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task RunCleanArticle(Func<IEnumerable<Article>, IEnumerable<Article>> articleFactory, Func<Article, Task> articleFunction)
        {
            try
            {
                int articlesCount;
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<NewsArticleContext>();
                    var allArticles = articleFactory(context.Articles);
                    articlesCount = allArticles.Count();
                }

                Console.WriteLine($"Processing {articlesCount} articles.");
                int articlesCleaned = 0;

                int scopeCount = (articlesCount / SplitArticleCount) + 1;
                foreach (var scopes in Enumerable.Range(0, scopeCount))
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var splitContext = scope.ServiceProvider.GetRequiredService<NewsArticleContext>();
                        var articleCleaner = scope.ServiceProvider.GetRequiredService<IArticleCleaner>();
                        var articles = articleFactory(splitContext.Articles).Skip(articlesCleaned).Take(SplitArticleCount);

                        foreach (var article in articles)
                        {
                            await articleFunction(article);

                            articlesCleaned++;
                            if (articlesCleaned % (articlesCount / 100) == 0)
                            {
                                Console.WriteLine($"{articlesCleaned} articles processed.");
                            }
                        }
                    }
                }
                Console.WriteLine($"{articlesCleaned} articles processed.");
                Console.WriteLine("Bulkk processing complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
