using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        /// <summary>
        /// Run a given delegate on articles matching the article predicate.
        /// </summary>
        /// <remarks>
        /// This will create and dispose contexts regularly, resulting in less memory usage.
        /// </remarks>
        /// <param name="articlePredicate">Articles to operate on.</param>
        /// <param name="articleFunction">Function to run on individual articles.</param>
        /// <returns></returns>
        public async Task RunArticleBatch(Expression<Func<Article, bool>> articlePredicate, Func<Article, Task<bool>> articleFunction)
        {
            try
            {
                int articlesCount;
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<NewsArticleContext>();
                    articlesCount = context.Articles.Count(articlePredicate);
                }

                Console.WriteLine($"Processing {articlesCount} articles.");
                int articlesProcessed = 0;

                int scopeCount = (articlesCount / SplitArticleCount) + 1;
                foreach (var scopes in Enumerable.Range(0, scopeCount))
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var splitContext = scope.ServiceProvider.GetRequiredService<NewsArticleContext>();
                        var articleCleaner = scope.ServiceProvider.GetRequiredService<IArticleCleaner>();
                        var articles = splitContext.Articles.Where(articlePredicate).Skip(articlesProcessed).Take(SplitArticleCount);
                        var articlesUpdateList = new List<Article>();

                        foreach (var article in articles)
                        {
                            var toUpdate = await articleFunction(article);
                            if (toUpdate)
                            {
                                articlesUpdateList.Add(article);
                            }

                            articlesProcessed++;
                            if (articlesProcessed % (articlesCount / 100) == 0)
                            {
                                Console.WriteLine($"{articlesProcessed} of {articlesCount} articles processed.");
                            }
                        }

                        if (articlesUpdateList.Any())
                        {
                            await splitContext.SaveChangesAsync();
                        }
                    }
                }
                Console.WriteLine($"{articlesProcessed} articles processed.");
                Console.WriteLine("Bulk processing complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
