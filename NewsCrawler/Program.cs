using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RichTea.CommandLineParser;
using System;
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
    }
}
