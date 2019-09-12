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
        private static int Main(string[] args)
        {
            Console.WriteLine("Starting News Crawler Migrator.");
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
        public static async Task RunCrawler()
        {
            var serviceProvider = ServiceProviderFactory.CreateGenericServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var newsArticleFetcherRunner = scope.ServiceProvider.GetRequiredService<INewsArticleFetcherRunner>();
                await newsArticleFetcherRunner.RunFetcher();
            }
        }

    }
}
