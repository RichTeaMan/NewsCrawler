using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        static ServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting BBC news crawler.");

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();

            var connectionString = config.GetConnectionString("NewsArticleDatabase");

            var serviceCollection = new ServiceCollection();

            var startup = new Startup(config);
            startup.ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();

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
            var newsArticleFetcherRunner = serviceProvider.GetRequiredService<INewsArticleFetcherRunner>();
            await newsArticleFetcherRunner.RunFetcher();
        }

        [ClCommand("Title-Update")]
        public static async Task RunTitleUpdate()
        {
            var titleUpdater = serviceProvider.GetRequiredService<ITitleUpdaterRunner>();
            await titleUpdater.RunTitleUpdater();
        }

        [ClCommand("Clean-Article")]
        public static async Task RunCleanArticle()
        {
            try
            {
                var context = serviceProvider.GetRequiredService<NewsArticleContext>();
                var articles = context.Articles.Where(a => !a.IsIndexPage && a.PublishedDate != null);
                var articleCleaner = serviceProvider.GetRequiredService<IArticleCleaner>();
                Directory.CreateDirectory("cleanedArticles");
                int articleCount = articles.Count();
                Console.WriteLine($"Cleaning {articleCount} articles.");
                int articlesCleaned = 0;
                foreach (var article in articles)
                {
                    string fileName = article.Title;
                    foreach(var invalidFilenameChar in Path.GetInvalidFileNameChars())
                    {
                        fileName = fileName.Replace(invalidFilenameChar.ToString(), string.Empty);
                    }
                    var clean = articleCleaner.CleanArticle(article);
                    await File.WriteAllTextAsync($"cleanedArticles/{fileName}.txt", clean);

                    article.Content = string.Empty;

                    articlesCleaned++;
                    if (articlesCleaned % (articleCount / 100) == 0)
                    {
                        Console.WriteLine($"{articlesCleaned} articles cleaned.");
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
