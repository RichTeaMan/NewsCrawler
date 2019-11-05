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

namespace NewsCrawler
{
    public abstract class AbstractArticleFinderService : INewsArticleFinderService
    {
        private readonly ILogger logger;

        private readonly INewsArticleDeterminationService newsArticleDeterminationService;

        protected abstract string BaseUrl { get; }

        protected abstract string[] FetchIndexUrls();

        public abstract string SourceName { get; }

        public AbstractArticleFinderService(ILogger logger, INewsArticleDeterminationService newsArticleDeterminationService)
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
            foreach (var indexUrl in FetchIndexUrls())
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
                .Distinct()
                .Where(v => newsArticleDeterminationService.IsNewsArticle(v))
                .ToArray();
            return links;
        }

        private string FindHref(HtmlNode htmlNode)
        {
            var href = htmlNode.Attributes.FirstOrDefault(attr => attr.Name == "href")?.Value;
            if (href?.StartsWith("//") == true)
            {
                href = href.Remove(0, 2);
            }
            else if (href?.StartsWith("/") == true)
            {
                href = $"{BaseUrl}{href}";
            }
            return href;
        }

    }
}
