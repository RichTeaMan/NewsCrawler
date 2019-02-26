using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Bbc;
using NewsCrawler.Cnn;
using NewsCrawler.DailyMail;
using NewsCrawler.Guardian;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System;
using System.Collections.Generic;

namespace NewsCrawler
{
    public class ServiceProviderFactory
    {
        public static IEnumerable<IServiceProvider> CreateServiceProviders()
        {
            yield return CreateBbcServiceProvider();
            yield return CreateDailyMailServiceProvider();
            yield return CreateGuardianServiceProvider();
            yield return CreateCnnServiceProvider();
        }

        public static IServiceProvider CreateBbcServiceProvider()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();
            var connectionString = config.GetConnectionString("NewsArticleDatabase");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            serviceCollection.AddScoped<INewsArticleDeterminationService, BbcNewsArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, BbcNewsArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            serviceCollection.AddScoped<INewsArticleFetchService, NewsArticleFetchService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, BbcNewsArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, BbcNewsArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, BbcArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, BbcArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateDailyMailServiceProvider()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();
            var connectionString = config.GetConnectionString("NewsArticleDatabase");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            serviceCollection.AddScoped<INewsArticleDeterminationService, DailyMailNewsArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, DailyMailNewsArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            serviceCollection.AddScoped<INewsArticleFetchService, NewsArticleFetchService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, DailyMailNewsArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, DailyMailNewsArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, DailyMailArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, DailyMailArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateGuardianServiceProvider()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();
            var connectionString = config.GetConnectionString("NewsArticleDatabase");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            serviceCollection.AddScoped<INewsArticleDeterminationService, GuardianArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, GuardianArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            serviceCollection.AddScoped<INewsArticleFetchService, NewsArticleFetchService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, GuardianArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, GuardianArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, GuardianArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, GuardianArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static IServiceProvider CreateCnnServiceProvider()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();
            var connectionString = config.GetConnectionString("NewsArticleDatabase");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            serviceCollection.AddScoped<INewsArticleDeterminationService, CnnArticleDeterminationService>();
            serviceCollection.AddScoped<INewsArticleFinderService, CnnArticleFinderService>();
            serviceCollection.AddScoped<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            serviceCollection.AddScoped<INewsArticleFetchService, NewsArticleFetchService>();
            serviceCollection.AddScoped<INewsArticleTitleFetcherService, CnnArticleTitleFetcherService>();
            serviceCollection.AddScoped<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            serviceCollection.AddScoped<IArticlePublishedDateFetcherService, CnnArticlePublishedDateFetcherService>();
            serviceCollection.AddScoped<IArticleCleaner, CnnArticleCleaner>();
            serviceCollection.AddScoped<IArticleCleanerRunner, CnnArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
