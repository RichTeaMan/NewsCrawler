using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Persistence;

namespace NewsCrawler
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<INewsArticleDeterminationService, NewsArticleDeterminationService>();
            services.AddSingleton<INewsArticleFinderService, BbcNewsArticleFinderService>();
            services.AddSingleton<INewsArticleFetcherRunner, NewsArticleFetcherRunner>();
            services.AddSingleton<INewsArticleFetchService, NewsArticleFetchService>();
            services.AddSingleton<INewsArticleTitleFetcherService, BbcNewsArticleTitleFetcherService>();
            services.AddSingleton<NewsArticleContext>();
        }
    }
}
