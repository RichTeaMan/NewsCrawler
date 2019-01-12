using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewsCrawler
{
    public class BbcNewsArticleFinderService : INewsArticleFinderService
    {
        private readonly INewsArticleDeterminationService newsArticleDeterminationService;

        private readonly string baseUrl = "https://www.bbc.co.uk";

        public BbcNewsArticleFinderService(INewsArticleDeterminationService newsArticleDeterminationService)
        {
            this.newsArticleDeterminationService = newsArticleDeterminationService ?? throw new ArgumentNullException(nameof(newsArticleDeterminationService));
        }

        public IEnumerable<string> FindNewsArticles()
        {
            var url = "https://www.bbc.co.uk/news/uk";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var links = doc.DocumentNode.Descendants()
                .Where(n => n.Name == "a")
                .Select(n => FindHref(n))
                .Where(v => newsArticleDeterminationService.IsNewsArticle(v))
                .Select(v => $"{baseUrl}{v}");
            return links;
        }

        private string FindHref(HtmlNode htmlNode)
        {
            return htmlNode.Attributes.FirstOrDefault(attr => attr.Name == "href")?.Value;
        }

    }
}
