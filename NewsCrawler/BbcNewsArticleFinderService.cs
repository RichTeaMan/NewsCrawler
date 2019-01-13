using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsCrawler
{
    public class BbcNewsArticleFinderService : INewsArticleFinderService
    {
        private readonly INewsArticleDeterminationService newsArticleDeterminationService;

        private readonly string baseUrl = "https://www.bbc.co.uk";

        private readonly string[] indexUrls = new[]
        {
            "https://www.bbc.co.uk/news/uk",
            "https://www.bbc.co.uk/news/politics",
            "https://www.bbc.co.uk/news/health",
            "https://www.bbc.co.uk/news/education",
            "https://www.bbc.co.uk/news/technology",
            "https://www.bbc.co.uk/news/science_and_environment",
            "https://www.bbc.co.uk/news/entertainment_and_arts",
            "https://www.bbc.co.uk/news/stories",
            "https://www.bbc.co.uk/news/popular/read"
        };

        public BbcNewsArticleFinderService(INewsArticleDeterminationService newsArticleDeterminationService)
        {
            this.newsArticleDeterminationService = newsArticleDeterminationService ?? throw new ArgumentNullException(nameof(newsArticleDeterminationService));
        }

        public IEnumerable<string> FindNewsArticles()
        {
            var docuemntNodes = new List<HtmlNode>();
            foreach (var indexUrl in indexUrls)
            {
                var web = new HtmlWeb();
                var doc = web.Load(indexUrl);
                docuemntNodes.Add(doc.DocumentNode);
            }

            var links = docuemntNodes.SelectMany(n => n.Descendants())
                .Where(n => n.Name == "a")
                .Select(n => FindHref(n))
                .Where(v => newsArticleDeterminationService.IsNewsArticle(v))
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
