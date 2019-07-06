using DocumentScanner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsCrawler.Persistence;
using NewsCrawler.WebUI.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler.WebUI.Controllers
{
    public class ArticleDetailController : Controller
    {
        private readonly NewsArticleContext newsArticleContext;

        private readonly DocumentScannerService documentScannerService;

        public ArticleDetailController(NewsArticleContext newsArticleContext, DocumentScannerService documentScannerService)
        {
            this.newsArticleContext = newsArticleContext ?? throw new ArgumentNullException(nameof(newsArticleContext));
            this.documentScannerService = documentScannerService ?? throw new ArgumentNullException(nameof(documentScannerService));
        }

        public async Task<IActionResult> Index(int id)
        {
            var article = await newsArticleContext.Articles
                .SingleOrDefaultAsync(a => a.Id == id);

            var scanResponse = await documentScannerService.ScanDocument(article.CleanedContent);
            var nouns = scanResponse.DocumentTokens
                .Where(t => t.IsProperNoun())
                .GroupBy(t => t.Text)
                .Select(g => new WordFrequency(g.Count(), g.Key))
                .OrderByDescending(wf => wf.Frequency);

            var articleDetail = new ArticleDetail(article, nouns);

            return View("Index", articleDetail);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
