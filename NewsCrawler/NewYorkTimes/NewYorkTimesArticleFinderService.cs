using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsCrawler.NewYorkTimes
{
    public class NewYorkTimesArticleFinderService : INewsArticleFinderService
    {
        private readonly INewsArticleDeterminationService newsArticleDeterminationService;

        private readonly string[] indexUrls = new[]
        {
            "https://www.nytimes.com/",
            "https://www.nytimes.com/section/education",
            "https://www.nytimes.com/section/automobiles",
            "https://www.nytimes.com/section/theater",
            "https://www.nytimes.com/section/learning",
            "https://www.nytimes.com/section/reader-center",
            "https://www.nytimes.com/section/jobs",
            "https://www.nytimes.com/section/fashion/weddings",
            "https://www.nytimes.com/section/fashion",
            "https://www.nytimes.com/section/lens",
            "https://www.nytimes.com/section/multimedia",
            "https://www.nytimes.com/column/book-review-podcast",
            "https://www.nytimes.com/section/corrections",
            "https://www.nytimes.com/column/childrens-books",
            "https://www.nytimes.com/section/opinion/columnists",
            "https://www.nytimes.com/column/by-the-book",
            "https://www.nytimes.com/column/crime",
            "https://www.nytimes.com/section/opinion/contributors",
            "https://www.nytimes.com/section/opinion/editorials",
            "https://www.nytimes.com/section/us",
            "https://www.nytimes.com/section/world",
            "https://www.nytimes.com/section/nyregion",
            "https://www.nytimes.com/section/politics",
            "https://www.nytimes.com/section/realestate",
            "https://www.nytimes.com/section/business",
            "https://www.nytimes.com/section/opinion",
            "https://www.nytimes.com/section/technology",
            "https://www.nytimes.com/section/science",
            "https://www.nytimes.com/section/health",
            "https://www.nytimes.com/section/sports",
            "https://www.nytimes.com/section/style",
            "https://www.nytimes.com/section/magazine",
            "https://www.nytimes.com/section/t-magazine",
            "https://www.nytimes.com/column/bookends",
            "https://www.nytimes.com/section/arts",
            "https://www.nytimes.com/section/food",
            "https://www.nytimes.com/section/travel",
            "https://www.nytimes.com/section/upshot",
            "https://www.nytimes.com/section/climate",
            "https://www.nytimes.com/section/opinion/letters",
            "https://www.nytimes.com/section/books",
            "https://www.nytimes.com/column/living-in",
            "https://www.nytimes.com/section/opinion/sunday",
            "https://www.nytimes.com/section/arts/design",
            "https://www.nytimes.com/section/arts/dance",
            "https://www.nytimes.com/section/obituaries",
            "https://www.nytimes.com/column/ask-real-estate",
            "https://www.nytimes.com/section/todayspaper",
            "https://www.nytimes.com/section/movies",
            "https://www.nytimes.com/section/arts/music",
            "https://www.nytimes.com/section/arts/television"
        };


        public NewYorkTimesArticleFinderService(INewsArticleDeterminationService newsArticleDeterminationService)
        {
            this.newsArticleDeterminationService = newsArticleDeterminationService ?? throw new ArgumentNullException(nameof(newsArticleDeterminationService));
        }

        public string SourceName => "New York Times";

        public IEnumerable<string> FindNewsArticles()
        {
            var triedPageUrls = new HashSet<string>();

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
                .Where(v => newsArticleDeterminationService.IsNewsArticle(v) || newsArticleDeterminationService.IsIndexPage(v))
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
