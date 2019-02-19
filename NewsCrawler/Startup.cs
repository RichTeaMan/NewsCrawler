using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Bbc;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;

namespace NewsCrawler
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(Configuration.GetConnectionString("NewsArticleDatabase")));

            services.AddSingleton<INewsArticleDeterminationService, BbcNewsArticleDeterminationService>();
            services.AddSingleton<INewsArticleFinderService, BbcNewsArticleFinderService>();
            services.AddSingleton<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            services.AddSingleton<INewsArticleFetchService, NewsArticleFetchService>();
            services.AddSingleton<INewsArticleTitleFetcherService, BbcNewsArticleTitleFetcherService>();
            services.AddSingleton<IArticleUpdaterRunner, ArticleUpdaterRunner>();
            services.AddSingleton<IIndexPageDeterminationService, BbcIndexPageDeterminationService>();
            services.AddSingleton<IArticlePublishedDateFetcherService, BbcNewsArticlePublishedDateFetcherService>();
            services.AddSingleton<IArticleCleaner, ArticleCleaner>();
        }
    }
}
