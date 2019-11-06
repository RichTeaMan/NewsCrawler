using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NewsCrawler.Persistence.Postgres
{
    public class ArticleBatcher
    {

        private readonly IServiceProvider serviceProvider;

        public int ConcurrentArticlePredicates { get; set; } = 1;

        public int SplitArticleCount { get; set; } = 200;

        /// <summary>
        /// Gets the current active context. Be extremely mindful when using this directly.
        /// </summary>
        public PostgresNewsArticleContext CurrentContext { get; private set; } = null;

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
                    var context = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>();
                    articlesCount = context.Articles.Count(articlePredicate);
                }

                Console.WriteLine($"Processing {articlesCount} articles.");
                int articlesProcessed = 0;

                int scopeCount = (articlesCount / SplitArticleCount) + 1;
                foreach (var scopes in Enumerable.Range(0, scopeCount))
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        CurrentContext = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>();
                        var articles = CurrentContext.Articles.Where(articlePredicate).OrderBy(a => a.Id).Skip(articlesProcessed).Take(SplitArticleCount);
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
                            await CurrentContext.SaveChangesAsync();
                        }
                        CurrentContext = null;
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
