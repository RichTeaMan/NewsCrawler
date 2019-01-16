using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
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

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();

            var connectionString = config.GetConnectionString("NewsArticleDatabase");

            var serviceCollection = new ServiceCollection();

            var startup = new Startup(config);
            startup.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var newsArticleFetcherRunner = serviceProvider.GetRequiredService<INewsArticleFetcherRunner>();
            await newsArticleFetcherRunner.RunFetcher();
        }
    }
}
