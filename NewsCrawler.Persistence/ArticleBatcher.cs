using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Persistence;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NewsCrawler.Persistence
{
    public class ArticleBatcher
    {
        private const int SplitArticleCount = 200;

        private readonly IServiceProvider serviceProvider;

        public int ConcurrentArticlePredicates { get; set; } = 1;

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
        /// <param name="articleFunction">Function to run on individual articles. Return true to prompt an update.</param>
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
                        var articles = splitContext.Articles.Where(articlePredicate).Skip(articlesProcessed).Take(SplitArticleCount);
                        var articlesUpdateList = new ConcurrentBag<Article>();

                        await articles.ParallelForEachAsync(async article =>
                        {
                            var toUpdate = await articleFunction(article);
                            if (toUpdate)
                            {
                                articlesUpdateList.Add(article);
                            }

                            int _count = Interlocked.Increment(ref articlesProcessed);
                            if (_count % (articlesCount / 100) == 0)
                            {
                                Console.WriteLine($"{_count} of {articlesCount} articles processed.");
                            }
                        }, ConcurrentArticlePredicates);

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
