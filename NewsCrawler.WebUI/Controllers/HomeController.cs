using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsCrawler.Persistence;
using NewsCrawler.Persistence.Postgres;
using NewsCrawler.WebUI.Models;

namespace NewsCrawler.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly PostgresNewsArticleContext newsArticleContext;

        private const int ArticlesPerPage = 100;

        public HomeController(PostgresNewsArticleContext newsArticleContext)
        {
            this.newsArticleContext = newsArticleContext ?? throw new ArgumentNullException(nameof(newsArticleContext));
        }

        public IActionResult Index(int page = 1, string searchTerm = null, string[] newsSources = null)
        {
            var resultNewsSources = newsArticleContext.Articles.Select(a => a.NewsSource).Distinct().ToArray();

            var articles = newsArticleContext.Articles
                .Where(a => !a.IsIndexPage)
                .Where(a => string.IsNullOrEmpty(searchTerm) || a.Title.Contains(searchTerm))
                .Where(a => newsSources == null || !newsSources.Any() || newsSources.Contains(a.NewsSource))
                .OrderByDescending(a => a.RecordedDate)
                .Select(a => new Models.Article {
                    Id = a.Id,
                    Title = a.Title,
                    Link = a.Url,
                    IsIndexPage = a.IsIndexPage,
                    RecordedDate = a.RecordedDate,
                    PublishedDate = a.PublishedDate,
                    NewsSource = a.NewsSource,
                    ContentLength = a.ContentLength,
                    CleanedContentLength = a.CleanedContentLength
                });

            var pagedArticles = articles
                .Skip((page - 1) * ArticlesPerPage)
                .Take(ArticlesPerPage)
                .ToArray();

            var articleResult = new ArticleResult
            {
                ArticleList = pagedArticles,
                NewsSources = resultNewsSources,
                SelectedNewsSources = newsSources,
                SearchTerm = searchTerm,
                Page = page,
                TotalPages = (articles.Count() / ArticlesPerPage) + 1,
                ArticleCount = articles.Count()
            };

            return View("Index", articleResult);
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
