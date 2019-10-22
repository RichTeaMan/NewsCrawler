using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NewsCrawler.DailyMail
{
    public class DailyMailNewsArticleFinderService : INewsArticleFinderService
    {
        private readonly ILogger logger;

        private readonly INewsArticleDeterminationService newsArticleDeterminationService;

        private readonly string baseUrl = "https://www.dailymail.co.uk";

        private readonly string[] indexUrls = new[]
        {
            "https://www.dailymail.co.uk/",
            "https://www.dailymail.co.uk/home/sitemap.html",
            "https://www.dailymail.co.uk/home/sitemaparchive/index.html",
            "https://www.dailymail.co.uk/topics",
            "https://www.dailymail.co.uk/mobile",
            "https://www.dailymail.co.uk/screensaver",
            "https://www.dailymail.co.uk/home/rssMenu.html",
            "https://www.dailymail.co.uk/ourpapers",
            "https://www.dailymail.co.uk/home/contactus",
            "https://www.dailymail.co.uk/home/videoarchive/index.html",
            "https://www.dailymail.co.uk/home/weather/index.html",
            "https://www.dailymail.co.uk/motoring/index.html",
            "https://www.dailymail.co.uk/property/index.html",
            "https://www.dailymail.co.uk/home/prmts/index.html",
            "https://www.dailymail.co.uk/home/index.html",
            "https://www.dailymail.co.uk/news/index.html",
            "https://www.dailymail.co.uk/ushome/index.html",
            "https://www.dailymail.co.uk/sport/index.html",
            "https://www.dailymail.co.uk/tvshowbiz/index.html",
            "https://www.dailymail.co.uk/auhome/index.html",
            "https://www.dailymail.co.uk/femail/index.html",
            "https://www.dailymail.co.uk/health/index.html",
            "https://www.dailymail.co.uk/sciencetech/index.html",
            "https://www.dailymail.co.uk/video/index.html",
            "https://www.dailymail.co.uk/travel/index.html",
            "https://www.dailymail.co.uk/dailymailtv/index.html",
            "https://www.dailymail.co.uk/home/latest/index.html",
            "https://www.dailymail.co.uk/news/worldnews/index.html",
            "https://www.dailymail.co.uk/home/event/index.html",
            "https://www.dailymail.co.uk/home/books/index.html",
            "https://www.dailymail.co.uk/money/index.html",
        };

        public string SourceName => "Daily Mail";

        public DailyMailNewsArticleFinderService(ILogger<DailyMailNewsArticleFinderService> logger, INewsArticleDeterminationService newsArticleDeterminationService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.newsArticleDeterminationService = newsArticleDeterminationService ?? throw new ArgumentNullException(nameof(newsArticleDeterminationService));
        }

        public async Task<Source> FetchSource(PostgresNewsArticleContext postgresNewsArticleContext)
        {
            var source = await postgresNewsArticleContext.Source.FirstOrDefaultAsync(s => s.Name == SourceName);
            if (source == null)
            {
                source = new Source()
                {
                    Name = SourceName
                };
                await postgresNewsArticleContext.Source.AddAsync(source);
                await postgresNewsArticleContext.SaveChangesAsync();
            }
            return source;
        }

        public IEnumerable<string> FindNewsArticles()
        {
            var documentNodes = new List<HtmlNode>();
            foreach (var indexUrl in indexUrls)
            {
                try
                {
                    var web = new HtmlWeb();
                    var doc = web.Load(indexUrl);
                    documentNodes.Add(doc.DocumentNode);
                }
                catch (WebException ex)
                {
                    logger.LogError(ex, $"Error fetching index page: '{indexUrl}'.");
                }
            }

            var links = documentNodes.SelectMany(n => n.Descendants())
                .Where(n => n.Name == "a")
                .Select(n => FindHref(n))
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Where(v => v.StartsWith("/") && (newsArticleDeterminationService.IsNewsArticle(v) || newsArticleDeterminationService.IsIndexPage(v)))
                .Select(v => $"{baseUrl}{v}")
                .Distinct()
                .ToArray();
            return links;
        }

        private string FindHref(HtmlNode htmlNode)
        {
            return htmlNode.Attributes.FirstOrDefault(attr => attr.Name == "href")?.Value;
        }

    }
}
