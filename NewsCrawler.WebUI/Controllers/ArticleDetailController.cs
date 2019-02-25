using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsCrawler.Persistence;
using NewsCrawler.WebUI.Models;

namespace NewsCrawler.WebUI.Controllers
{
    public class ArticleDetailController : Controller
    {
        private readonly NewsArticleContext newsArticleContext;

        public ArticleDetailController(NewsArticleContext newsArticleContext)
        {
            this.newsArticleContext = newsArticleContext ?? throw new ArgumentNullException(nameof(newsArticleContext));
        }

        public async Task<IActionResult> Index(int id)
        {
            var article = await newsArticleContext.Articles
                .SingleOrDefaultAsync(a => a.Id == id);

            var articleDetail = new ArticleDetail(article);

            return View("Index", articleDetail);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
