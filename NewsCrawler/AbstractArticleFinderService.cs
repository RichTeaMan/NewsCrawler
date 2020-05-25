using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
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
            var documentLinks = new ConcurrentBag<string>();
            var indexUrls = FetchIndexUrls();
            logger.LogInformation($"Fetching {indexUrls.Length} index pages.");
            int i = 0;
            Parallel.ForEach(indexUrls, indexUrl =>
            {
                try
                {
                    var web = new HtmlWeb();
                    var doc = web.Load(indexUrl);
                    var docLinks = doc.DocumentNode.Descendants()
                        .Where(n => n.Name == "a")
                        .Select(n => FindHref(n))
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Distinct()
                        .ToList();

                    docLinks.ForEach(l => documentLinks.Add(l));
                }
                catch (WebException ex)
                {
                    logger.LogError(ex, $"Error fetching index page: '{indexUrl}'.");
                }
                int _i = Interlocked.Increment(ref i);
                logger.LogInformation($"Index page {_i} of {indexUrls.Length} fetched.");
            });

            var links = documentLinks
                .Distinct()
                .Where(v => newsArticleDeterminationService.IsNewsArticle(v) || newsArticleDeterminationService.IsIndexPage(v))
                .ToArray();
            return links;
        }

        private string FindHref(HtmlNode htmlNode)
        {
            var href = htmlNode.Attributes.FirstOrDefault(attr => attr.Name == "href")?.Value;
            if (href != null)
            {
                if (href.StartsWith("//"))
                {
                    href = href.Remove(0, 2);
                }
                else if (href.StartsWith("/"))
                {
                    href = $"{BaseUrl}{href}";
                }

                // remove query params
                int questionIndex = href.IndexOf('?');
                if (questionIndex != -1)
                {
                    href = href.Substring(0, questionIndex);
                }

                // remove anchors
                int hashIndex = href.IndexOf('#');
                if (hashIndex != -1)
                {
                    href = href.Substring(0, hashIndex);
                }

                if (href.EndsWith("/"))
                {
                    href = href.TrimEnd('/');
                }
            }
            return href;
        }

    }
}
