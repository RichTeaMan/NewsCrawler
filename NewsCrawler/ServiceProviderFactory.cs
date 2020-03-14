using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsCrawler.Bbc;
using NewsCrawler.Cnn;
using NewsCrawler.DailyMail;
using NewsCrawler.Guardian;
using NewsCrawler.Interfaces;
using NewsCrawler.NewYorkTimes;
using NewsCrawler.Persistence;
using NewsCrawler.Persistence.Postgres;
using Npgsql;
using System;
using System.Collections.Generic;

namespace NewsCrawler
{
    public class ServiceProviderFactory
    {
        /// <summary>
        /// Creates generised services for dealing with articles. Some services not do have generic version
        /// so some service lookup will not be resolved.
        /// </summary>
        /// <returns></returns>
        public static IServiceProvider CreateGenericServiceProvider(string databaseString)
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection, databaseString);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        private static void AddGenericServicesToCollection(ServiceCollection serviceCollection, string databaseString)
        {
            var inMemoryCommand = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(databaseString))
            {
                inMemoryCommand.Add("ConnectionStrings:PostgresNewsArticleDatabase", databaseString);
            }

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddInMemoryCollection(inMemoryCommand);
            var config = builder.Build();

            var postgresConnectionString = config.GetConnectionString("PostgresNewsArticleDatabase");
            try
            {

                var postgresConnection = new NpgsqlConnectionStringBuilder(postgresConnectionString);
                Console.WriteLine($"Database host: {postgresConnection.Host}");
                Console.WriteLine($"Database name: {postgresConnection.Database}");
                Console.WriteLine($"Database username: {postgresConnection.Username}");
            }
            catch (Exception ex)
            {
                throw new Exception("Could not parse connection string.", ex);
            }
            serviceCollection.AddDbContext<PostgresNewsArticleContext>(options => options.UseNpgsql(postgresConnectionString,
                pgOptions => pgOptions.CommandTimeout(120)),
                ServiceLifetime.Transient);

            serviceCollection.AddLogging(configure => { configure.AddConfiguration(config.GetSection("Logging")); configure.AddConsole(); });
            serviceCollection.AddScoped<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            serviceCollection.AddScoped<INewsArticleFetchService, NewsArticleFetchService>();
            serviceCollection.AddScoped<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            serviceCollection.AddScoped<IWordCountService, SpacyWordCountService>();
            serviceCollection.AddSingleton<CrawlerRunner>();


        }

        public static IEnumerable<IServiceProvider> CreateServiceProviders(string databaseString)
        {
            yield return CreateBbcServiceProvider(databaseString);
            yield return CreateDailyMailServiceProvider(databaseString);
            yield return CreateGuardianServiceProvider(databaseString);
            yield return CreateCnnServiceProvider(databaseString);
            yield return CreateNewYorkTimesServiceProvider(databaseString);
        }

        public static IServiceProvider CreateBbcServiceProvider(string databaseString)
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection, databaseString);

            serviceCollection.AddScoped<INewsArticleDeterminationService, BbcNewsArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, BbcNewsArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, BbcNewsArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, BbcNewsArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, BbcArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, BbcArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateDailyMailServiceProvider(string databaseString)
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection, databaseString);

            serviceCollection.AddScoped<INewsArticleDeterminationService, DailyMailNewsArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, DailyMailNewsArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, DailyMailNewsArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, DailyMailNewsArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, DailyMailArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, DailyMailArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateGuardianServiceProvider(string databaseString)
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection, databaseString);

            serviceCollection.AddScoped<INewsArticleDeterminationService, GuardianArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, GuardianArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, GuardianArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, GuardianArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, GuardianArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, GuardianArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateCnnServiceProvider(string databaseString)
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection, databaseString);

            serviceCollection.AddScoped<INewsArticleDeterminationService, CnnArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, CnnArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, CnnArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, CnnArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, CnnArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, CnnArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateNewYorkTimesServiceProvider(string databaseString)
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection, databaseString);

            serviceCollection.AddScoped<INewsArticleDeterminationService, NewYorkTimesArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, NewYorkTimesArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, NewYorkTimesArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, NewYorkTimesArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, NewYorkTimesArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, NewYorkTimesArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
