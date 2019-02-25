﻿using Microsoft.EntityFrameworkCore;
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
            serviceCollection.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<INewsArticleDeterminationService, BbcNewsArticleDeterminationService>();
            serviceCollection.AddSingleton<INewsArticleFinderService, BbcNewsArticleFinderService>();
            serviceCollection.AddSingleton<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            serviceCollection.AddSingleton<INewsArticleFetchService, NewsArticleFetchService>();
            serviceCollection.AddSingleton<INewsArticleTitleFetcherService, BbcNewsArticleTitleFetcherService>();
            serviceCollection.AddSingleton<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            serviceCollection.AddSingleton<IArticlePublishedDateFetcherService, BbcNewsArticlePublishedDateFetcherService>();
            serviceCollection.AddSingleton<IArticleCleaner, BbcArticleCleaner>();
            serviceCollection.AddSingleton<IArticleCleanerRunner, BbcArticleCleanerRunner>();

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
            serviceCollection.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<INewsArticleDeterminationService, DailyMailNewsArticleDeterminationService>();
            serviceCollection.AddSingleton<INewsArticleFinderService, DailyMailNewsArticleFinderService>();
            serviceCollection.AddSingleton<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            serviceCollection.AddSingleton<INewsArticleFetchService, NewsArticleFetchService>();
            serviceCollection.AddSingleton<INewsArticleTitleFetcherService, DailyMailNewsArticleTitleFetcherService>();
            serviceCollection.AddSingleton<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            serviceCollection.AddSingleton<IArticlePublishedDateFetcherService, DailyMailNewsArticlePublishedDateFetcherService>();
            serviceCollection.AddSingleton<IArticleCleaner, DailyMailArticleCleaner>();
            serviceCollection.AddSingleton<IArticleCleanerRunner, DailyMailArticleCleanerRunner>();

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
            serviceCollection.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<INewsArticleDeterminationService, GuardianArticleDeterminationService>();
            serviceCollection.AddSingleton<INewsArticleFinderService, GuardianArticleFinderService>();
            serviceCollection.AddSingleton<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            serviceCollection.AddSingleton<INewsArticleFetchService, NewsArticleFetchService>();
            serviceCollection.AddSingleton<INewsArticleTitleFetcherService, GuardianArticleTitleFetcherService>();
            serviceCollection.AddSingleton<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            serviceCollection.AddSingleton<IArticlePublishedDateFetcherService, GuardianArticlePublishedDateFetcherService>();
            serviceCollection.AddSingleton<IArticleCleaner, GuardianArticleCleaner>();
            serviceCollection.AddSingleton<IArticleCleanerRunner, GuardianArticleCleanerRunner>();

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
            serviceCollection.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<INewsArticleDeterminationService, CnnArticleDeterminationService>();
            serviceCollection.AddSingleton<INewsArticleFinderService, CnnArticleFinderService>();
            serviceCollection.AddSingleton<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            serviceCollection.AddSingleton<INewsArticleFetchService, NewsArticleFetchService>();
            serviceCollection.AddSingleton<INewsArticleTitleFetcherService, CnnArticleTitleFetcherService>();
            serviceCollection.AddSingleton<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            serviceCollection.AddSingleton<IArticlePublishedDateFetcherService, CnnArticlePublishedDateFetcherService>();
            serviceCollection.AddSingleton<IArticleCleaner, CnnArticleCleaner>();
            serviceCollection.AddSingleton<IArticleCleanerRunner, CnnArticleCleanerRunner>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
