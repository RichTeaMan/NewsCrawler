using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        /// Gets or sets if content and cleaned content should be included in the batch.
        /// </summary>
        public bool IncludeContent { get; set; } = false;

        /// <summary>
        /// Gets or sets if entity IDs should be entirely determined before writing any changes.
        /// </summary>
        /// <remarks>
        /// This should be set to true if the article function is expected to make changes that will change the result
        /// of the predicate. If this is not done the function is likely to skip over some entities as the predicate
        /// will find different entities for every iteration. However, this is likely to come wit performance penalties.
        /// </remarks>
        public bool PreLoadEntityIds { get; set; } = false;

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
                var articleIds = new List<int>();
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>();
                    articlesCount = context.Articles.Count(articlePredicate);
                    Console.WriteLine($"Processing {articlesCount} articles.");
                    if (PreLoadEntityIds)
                    {
                        Console.WriteLine("Getting articles IDs...");
                        articleIds.AddRange(context.Articles.Where(articlePredicate).Select(a => a.Id).OrderBy(a => a));
                        Console.WriteLine("Fetched article IDs.");
                    }
                }


                int articlesProcessed = 0;

                int scopeCount = (articlesCount / SplitArticleCount) + 1;
                foreach (var scopes in Enumerable.Range(0, scopeCount))
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        CurrentContext = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>();

                        IQueryable<Article> articlesToFilter;
                        if (IncludeContent)
                        {
                            articlesToFilter = CurrentContext.Articles
                                .Include(a => a.Source)
                                .Include(a => a.ArticleContent)
                                .Include(a => a.ArticleCleanedContent);
                        }
                        else
                        {
                            articlesToFilter = CurrentContext.Articles
                                .Include(a => a.Source);
                        }

                        IQueryable<Article> articles;
                        if (PreLoadEntityIds)
                        {
                            var ids = articleIds.Skip(articlesProcessed).Take(SplitArticleCount);
                            articles = articlesToFilter.Where(a => ids.Contains(a.Id)).OrderBy(a => a.Id);
                        }
                        else
                        {
                            articles = articlesToFilter.Where(articlePredicate).OrderBy(a => a.Id).Skip(articlesProcessed).Take(SplitArticleCount);
                        }
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
