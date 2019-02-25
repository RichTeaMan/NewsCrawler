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
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting News Crawler.");
            ParseCommand(args);
        }

        private static void ParseCommand(string[] args)
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
            foreach (var serviceProvider in ServiceProviderFactory.CreateServiceProviders())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var newsArticleFetcherRunner = scope.ServiceProvider.GetRequiredService<INewsArticleFetcherRunner>();
                    await newsArticleFetcherRunner.RunFetcher();
                }
            }
        }

        [ClCommand("Title-Update")]
        public static async Task RunTitleUpdate()
        {
            using (var scope = ServiceProviderFactory.CreateBbcServiceProvider().CreateScope())
            {
                var titleUpdater = scope.ServiceProvider.GetRequiredService<IArticleUpdaterRunner>();
                await titleUpdater.RunTitleUpdater("bbc.co.uk");
            }

            using (var scope = ServiceProviderFactory.CreateDailyMailServiceProvider().CreateScope())
            {
                var titleUpdater = scope.ServiceProvider.GetRequiredService<IArticleUpdaterRunner>();
                await titleUpdater.RunTitleUpdater("dailymail.co.uk");
            }

            using (var scope = ServiceProviderFactory.CreateGuardianServiceProvider().CreateScope())
            {
                var titleUpdater = scope.ServiceProvider.GetRequiredService<IArticleUpdaterRunner>();
                await titleUpdater.RunTitleUpdater("www.theguardian.com");
            }

            using (var scope = ServiceProviderFactory.CreateCnnServiceProvider().CreateScope())
            {
                var titleUpdater = scope.ServiceProvider.GetRequiredService<IArticleUpdaterRunner>();
                await titleUpdater.RunTitleUpdater("edition.cnn.com");
            }
        }

        [ClCommand("Clean-Article")]
        public static async Task RunCleanArticle()
        {
            foreach (var serviceProvider in ServiceProviderFactory.CreateServiceProviders())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var newsArticleCleanerRunner = scope.ServiceProvider.GetRequiredService<IArticleCleanerRunner>();
                    await newsArticleCleanerRunner.CleanArticles();
                }
            }
        }
    }
}
