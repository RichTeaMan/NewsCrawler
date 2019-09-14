using DocumentScanner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsCrawler.Persistence;
using NewsCrawler.Persistence.Postgres;
using NewsCrawler.WebUI.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler.WebUI.Controllers
{
    public class StatsController : Controller
    {
        private readonly ILogger logger;

        private readonly NewsArticleContext newsArticleContext;

        private readonly PostgresNewsArticleContext postgresNewsArticleContext;

        public StatsController(ILogger<StatsController> logger, NewsArticleContext newsArticleContext, PostgresNewsArticleContext postgresNewsArticleContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.newsArticleContext = newsArticleContext ?? throw new ArgumentNullException(nameof(newsArticleContext));
            this.postgresNewsArticleContext = postgresNewsArticleContext ?? throw new ArgumentNullException(nameof(postgresNewsArticleContext));
        }

        public async Task<IActionResult> Index()
        {
            int postgresCount = await postgresNewsArticleContext.Articles.CountAsync();
            int sqlCount = await newsArticleContext.Articles.CountAsync();
            var latestPostgres = await postgresNewsArticleContext.Articles.MaxAsync(a => a.RecordedDate);
            var latestSql = await newsArticleContext.Articles.MaxAsync(a => a.RecordedDate);

            var stats = new Stats
            {
                PostgresArticles = postgresCount,
                SqlServerArticles = sqlCount,
                LatestPostgresArticle = latestPostgres,
                LatestSqlServerArticle = latestSql
            };
            return View("Index", stats);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
