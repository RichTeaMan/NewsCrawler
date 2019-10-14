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
    public class ArticleDetailController : Controller
    {
        private readonly ILogger logger;

        private readonly PostgresNewsArticleContext newsArticleContext;

        private readonly DocumentScannerService documentScannerService;

        public ArticleDetailController(
            ILogger<ArticleDetailController> logger,
            PostgresNewsArticleContext newsArticleContext,
            DocumentScannerService documentScannerService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.newsArticleContext = newsArticleContext ?? throw new ArgumentNullException(nameof(newsArticleContext));
            this.documentScannerService = documentScannerService ?? throw new ArgumentNullException(nameof(documentScannerService));
        }

        public async Task<IActionResult> Index(int id)
        {
            IActionResult result;
            var article = await newsArticleContext.Articles
                .Include(a => a.Source)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                result = NotFound();
            }
            else
            {
                var articleDetail = new ArticleDetail(article);

                try
                {
                    var scanResponse = await documentScannerService.ScanDocument(article.CleanedContent);
                    var nouns = scanResponse.DocumentTokens
                        .Where(t => t.IsProperNoun())
                        .GroupBy(t => t.Text)
                        .Select(g => new WordFrequency(g.Count(), g.Key))
                        .OrderByDescending(wf => wf.Frequency)
                        .ToArray();

                    articleDetail.Nouns = nouns;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed document scanning for article {id}.");
                    articleDetail.DocumentScannerError = true;
                }
                result = View("Index", articleDetail);
            }
            return result;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
