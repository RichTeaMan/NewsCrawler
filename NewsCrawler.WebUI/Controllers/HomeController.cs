using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsCrawler.Persistence;
using NewsCrawler.WebUI.Models;

namespace NewsCrawler.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly NewsArticleContext newsArticleContext;

        public HomeController(NewsArticleContext newsArticleContext)
        {
            this.newsArticleContext = newsArticleContext ?? throw new ArgumentNullException(nameof(newsArticleContext));
        }

        public IActionResult Index()
        {
            var articles = newsArticleContext.Articles.Select(a => new Models.Article { Title = a.Title, Link = a.Url, RecordedDate = a.RecordedDate }).ToArray();
            var articleResult = new ArticleResult
            {
                ArticleList = articles
            };

            return View(articleResult);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Search(string term)
        {
            var articles = newsArticleContext.Articles
                .Where(a => a.Title.Contains(term))
                .Select(a => new Models.Article { Title = a.Title, Link = a.Url, RecordedDate = a.RecordedDate })
                .ToArray();

            var articleResult = new ArticleResult
            {
                ArticleList = articles,
                SearchTerm = term
            };

            return View("Index", articleResult);
        }
    }
}
