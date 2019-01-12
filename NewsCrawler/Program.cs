using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewsCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting BBC news crawler.");

            var serviceCollection = new ServiceCollection();

            var startup = new Startup();
            startup.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var newsArticleFetcherRunner = serviceProvider.GetRequiredService<INewsArticleFetcherRunner>();
            await newsArticleFetcherRunner.RunFetcher();
        }
    }
}
