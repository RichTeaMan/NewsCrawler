using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using RichTea.CommandLineParser;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting News Crawler.");
            ParseCommand(args);
        }

        static void ParseCommand(string[] args)
        {
            MethodInvoker command = null;
            try
            {
                command = new CommandLineParserInvoker().GetCommand(typeof(Program), args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing command:");
                Console.WriteLine(ex);
            }
            if (command != null)
            {
                try
                {
                    command.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error running command:");
                    Console.WriteLine(ex);

                    var inner = ex.InnerException;
                    while (inner != null)
                    {
                        Console.WriteLine(inner);
                        Console.WriteLine();
                        inner = inner.InnerException;
                    }

                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        [DefaultClCommand]
        public static async Task RunCrawler()
        {
            using (var scope = ServiceProviderFactory.CreateBbcServiceProvider().CreateScope())
            {
                var newsArticleFetcherRunner = scope.ServiceProvider.GetRequiredService<INewsArticleFetcherRunner>();
                await newsArticleFetcherRunner.RunFetcher();
            }

            using (var scope = ServiceProviderFactory.CreateDailyMailServiceProvider().CreateScope())
            {
                var newsArticleFetcherRunner = scope.ServiceProvider.GetRequiredService<INewsArticleFetcherRunner>();
                await newsArticleFetcherRunner.RunFetcher();
            }
        }

        [ClCommand("Title-Update")]
        public static async Task RunTitleUpdate()
        {
            var serviceProvider = ServiceProviderFactory.CreateBbcServiceProvider();
            var titleUpdater = serviceProvider.GetRequiredService<IArticleUpdaterRunner>();
            await titleUpdater.RunTitleUpdater();
        }

        [ClCommand("Clean-Article")]
        public static async Task RunCleanArticle()
        {
            var serviceProvider = ServiceProviderFactory.CreateBbcServiceProvider();
            try
            {
                int splitArticleCount = 200;
                var context = serviceProvider.GetRequiredService<NewsArticleContext>();
                var articlesCount = context.Articles.Count(a => !a.IsIndexPage && a.PublishedDate != null);
                Directory.CreateDirectory("cleanedArticles");
                Console.WriteLine($"Cleaning {articlesCount} articles.");
                int articlesCleaned = 0;

                int scopeCount = (articlesCount / splitArticleCount) + 1;
                foreach (var scopes in Enumerable.Range(0, scopeCount))
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var splitContext = scope.ServiceProvider.GetRequiredService<NewsArticleContext>();
                        var articleCleaner = scope.ServiceProvider.GetRequiredService<IArticleCleaner>();
                        var articles = splitContext.Articles.Skip(articlesCleaned).Take(splitArticleCount);

                        foreach (var article in articles)
                        {
                            string fileName = article.Title;
                            foreach (var invalidFilenameChar in Path.GetInvalidFileNameChars())
                            {
                                fileName = fileName.Replace(invalidFilenameChar.ToString(), string.Empty);
                            }
                            var clean = articleCleaner.CleanArticle(article);
                            await File.WriteAllTextAsync($"cleanedArticles/{fileName}.txt", clean);

                            articlesCleaned++;
                            if (articlesCleaned % (articlesCount / 100) == 0)
                            {
                                Console.WriteLine($"{articlesCleaned} articles cleaned.");
                            }
                        }
                    }
                }
                Console.WriteLine($"{articlesCleaned} articles cleaned.");
                Console.WriteLine("Cleaing complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
