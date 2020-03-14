using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using RichTea.CommandLineParser;
using System;
using System.Threading.Tasks;

namespace NewsCrawler
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Console.WriteLine("Starting News Crawler.");
            return ParseCommand(args);
        }

        private static int ParseCommand(string[] args)
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
                    return 0;
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

                    return 1;
                }
            }
            return -1;
        }

        [DefaultClCommand]
        public static async Task RunCrawler([ClArgs("databaseString", "db")] string databaseString = null)
        {
            var crawlerRunner = ServiceProviderFactory.CreateGenericServiceProvider(databaseString).GetRequiredService<CrawlerRunner>();
            await crawlerRunner.RunCrawler(databaseString);
        }

        [ClCommand("Title-Update")]
        public static async Task RunTitleUpdate([ClArgs("databaseString", "db")] string databaseString = null)
        {
            using (var scope = ServiceProviderFactory.CreateBbcServiceProvider(databaseString).CreateScope())
            {
                var titleUpdater = scope.ServiceProvider.GetRequiredService<IArticleUpdaterRunner>();
                await titleUpdater.RunTitleUpdater("bbc.co.uk");
            }

            using (var scope = ServiceProviderFactory.CreateDailyMailServiceProvider(databaseString).CreateScope())
            {
                var titleUpdater = scope.ServiceProvider.GetRequiredService<IArticleUpdaterRunner>();
                await titleUpdater.RunTitleUpdater("dailymail.co.uk");
            }

            using (var scope = ServiceProviderFactory.CreateGuardianServiceProvider(databaseString).CreateScope())
            {
                var titleUpdater = scope.ServiceProvider.GetRequiredService<IArticleUpdaterRunner>();
                await titleUpdater.RunTitleUpdater("www.theguardian.com");
            }

            using (var scope = ServiceProviderFactory.CreateCnnServiceProvider(databaseString).CreateScope())
            {
                var titleUpdater = scope.ServiceProvider.GetRequiredService<IArticleUpdaterRunner>();
                await titleUpdater.RunTitleUpdater("edition.cnn.com");
            }
        }

        [ClCommand("Clean-Article")]
        public static async Task RunCleanArticle([ClArgs("databaseString", "db")] string databaseString = null)
        {
            foreach (var serviceProvider in ServiceProviderFactory.CreateServiceProviders(databaseString))
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var newsArticleCleanerRunner = scope.ServiceProvider.GetRequiredService<IArticleCleanerRunner>();
                    await newsArticleCleanerRunner.CleanArticles();
                }
            }
        }

        [ClCommand("Update-Word-Count")]
        public static async Task RunWordCount([ClArgs("databaseString", "db")] string databaseString = null)
        {
            using (var scope = ServiceProviderFactory.CreateGenericServiceProvider(databaseString).CreateScope())
            {
                var wordCountService = scope.ServiceProvider.GetRequiredService<IWordCountService>();
                await wordCountService.UpdateWordCount();
            }
        }

        [ClCommand("SetupTest")]
        public static async Task<int> SetupTest([ClArgs("databaseString", "db")] string databaseString = null)
        {
            using (var scope = ServiceProviderFactory.CreateGenericServiceProvider(databaseString).CreateScope())
            {
                Console.WriteLine("Setup successful.");

                Console.WriteLine("Running migration...");

                try
                {
                    using (var context = scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>())
                    {
                        await context.Database.MigrateAsync();
                    }

                    Console.WriteLine("Migration successful.");
                    return 0;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Migration failed.");
                    Console.Write(ex);
                    return 1;
                }
            }
        }
    }
}
