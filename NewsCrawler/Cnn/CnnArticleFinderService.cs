using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsCrawler.Cnn
{
    public class CnnArticleFinderService : INewsArticleFinderService
    {
        private readonly INewsArticleDeterminationService newsArticleDeterminationService;

        private readonly string baseUrl = "https://edition.cnn.com/";

        private readonly string[] indexUrls = new[]
        {
            "https://edition.cnn.com/",
        };

        public CnnArticleFinderService(INewsArticleDeterminationService newsArticleDeterminationService)
        {
            this.newsArticleDeterminationService = newsArticleDeterminationService ?? throw new ArgumentNullException(nameof(newsArticleDeterminationService));
        }

        public string SourceName => "CNN";

        public IEnumerable<string> FindNewsArticles()
        {
            var documentNodes = new List<HtmlNode>();
            foreach (var indexUrl in indexUrls)
            {
                var web = new HtmlWeb();
                var doc = web.Load(indexUrl);
                documentNodes.Add(doc.DocumentNode);
            }

            var links = documentNodes.SelectMany(n => n.Descendants())
                .Where(n => n.Name == "a")
                .Select(n => FindHref(n))
                .Where(v => newsArticleDeterminationService.IsNewsArticle(v) || newsArticleDeterminationService.IsIndexPage(v))
                .Select(v => MakeAbsolute(v))
                .Distinct()
                .ToArray();
            return links;
        }

        private string MakeAbsolute(string path)
        {
            if (path.StartsWith("/"))
            {
                path = $"{baseUrl}{path}";
            }
            return path;
        }

        private string FindHref(HtmlNode htmlNode)
        {
            return htmlNode.Attributes.FirstOrDefault(attr => attr.Name == "href")?.Value;
        }

    }
}
