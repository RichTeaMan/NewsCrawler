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
        public static IServiceProvider CreateGenericServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        private static void AddGenericServicesToCollection(ServiceCollection serviceCollection)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            var config = builder.Build();

            var postgresConnectionString = config.GetConnectionString("PostgresNewsArticleDatabase");
            serviceCollection.AddDbContext<PostgresNewsArticleContext>(options => options.UseNpgsql(postgresConnectionString,
                pgOptions => pgOptions.CommandTimeout(120)),
                ServiceLifetime.Transient);
            
            serviceCollection.AddLogging(configure => { configure.AddConfiguration(config.GetSection("Logging")); configure.AddConsole(); });
            serviceCollection.AddScoped<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            serviceCollection.AddScoped<INewsArticleFetchService, NewsArticleFetchService>();
            serviceCollection.AddScoped<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            serviceCollection.AddScoped<IWordCountService, SpacyWordCountService>();
        }

        public static IEnumerable<IServiceProvider> CreateServiceProviders()
        {
            yield return CreateBbcServiceProvider();
            yield return CreateDailyMailServiceProvider();
            yield return CreateGuardianServiceProvider();
            yield return CreateCnnServiceProvider();
            yield return CreateNewYorkTimesServiceProvider();
        }

        public static IServiceProvider CreateBbcServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection);

            serviceCollection.AddScoped<INewsArticleDeterminationService, BbcNewsArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, BbcNewsArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, BbcNewsArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, BbcNewsArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, BbcArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, BbcArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateDailyMailServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection);

            serviceCollection.AddScoped<INewsArticleDeterminationService, DailyMailNewsArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, DailyMailNewsArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, DailyMailNewsArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, DailyMailNewsArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, DailyMailArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, DailyMailArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateGuardianServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection);

            serviceCollection.AddScoped<INewsArticleDeterminationService, GuardianArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, GuardianArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, GuardianArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, GuardianArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, GuardianArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, GuardianArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateCnnServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection);

            serviceCollection.AddScoped<INewsArticleDeterminationService, CnnArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, CnnArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, CnnArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, CnnArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, CnnArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, CnnArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateNewYorkTimesServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            AddGenericServicesToCollection(serviceCollection);

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
