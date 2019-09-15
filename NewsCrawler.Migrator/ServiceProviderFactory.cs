using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using NewsCrawler.Persistence.Postgres;
using System;

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
            var sqlServerConnectionString = config.GetConnectionString("NewsArticleDatabase");
            var postgresConnectionString = config.GetConnectionString("PostgresNewsArticleDatabase");

            serviceCollection.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(sqlServerConnectionString,
                sqlServerOptions => sqlServerOptions.CommandTimeout(120 * 2)),
                ServiceLifetime.Transient);
            serviceCollection.AddDbContext<PostgresNewsArticleContext>(options => options.UseNpgsql(postgresConnectionString,
                contextOptions => contextOptions.CommandTimeout(120 * 2)),
                ServiceLifetime.Transient);
            serviceCollection.AddScoped<INewsArticleMigratorRunner, NewsArticleMigratorRunner>();
        }

    }
}
