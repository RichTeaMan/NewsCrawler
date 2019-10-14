using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler.Guardian
{
    public class GuardianArticleFinderService : INewsArticleFinderService
    {
        private readonly INewsArticleDeterminationService newsArticleDeterminationService;

        private readonly string baseUrl = "https://www.theguardian.com/";

        private readonly string[] indexUrls = new[]
        {
            "https://www.theguardian.com/uk",
            "https://www.theguardian.com/uk-news",
            "https://www.theguardian.com/world",
            "https://www.theguardian.com/uk/business",
            "https://www.theguardian.com/football",
            "https://www.theguardian.com/politics",
            "https://www.theguardian.com/uk/environment",
            "https://www.theguardian.com/education",
            "https://www.theguardian.com/society",
            "https://www.theguardian.com/science",
            "https://www.theguardian.com/uk/technology",
            "https://www.theguardian.com/global-development",
            "https://www.theguardian.com/cities",
            "https://www.theguardian.com/tone/obituaries",
            "https://www.theguardian.com/sport/rugby-union",
            "https://www.theguardian.com/sport/cricket",
            "https://www.theguardian.com/sport/tennis",
            "https://www.theguardian.com/sport/cycling",
            "https://www.theguardian.com/sport/formulaone",
            "https://www.theguardian.com/sport/golf",
            "https://www.theguardian.com/sport/boxing",
            "https://www.theguardian.com/sport/rugbyleague",
            "https://www.theguardian.com/sport/horse-racing",
            "https://www.theguardian.com/sport/us-sport",
            "https://www.theguardian.com/uk/film",
            "https://www.theguardian.com/music",
            "https://www.theguardian.com/uk/tv-and-radio",
            "https://www.theguardian.com/books",
            "https://www.theguardian.com/artanddesign",
            "https://www.theguardian.com/stage",
            "https://www.theguardian.com/games",
            "https://www.theguardian.com/music/classical-music-and-opera",
            "https://www.theguardian.com/fashion",
            "https://www.theguardian.com/food",
            "https://www.theguardian.com/tone/recipes",
            "https://www.theguardian.com/uk/travel",
            "https://www.theguardian.com/lifeandstyle/health-and-wellbeing",
            "https://www.theguardian.com/lifeandstyle/women",
            "https://www.theguardian.com/lifeandstyle/men",
            "https://www.theguardian.com/lifeandstyle/love-and-sex",
            "https://www.theguardian.com/fashion/beauty",
            "https://www.theguardian.com/lifeandstyle/home-and-garden",
            "https://www.theguardian.com/uk/money",
            "https://www.theguardian.com/technology/motoring"
        };

        public GuardianArticleFinderService(INewsArticleDeterminationService newsArticleDeterminationService)
        {
            this.newsArticleDeterminationService = newsArticleDeterminationService ?? throw new ArgumentNullException(nameof(newsArticleDeterminationService));
        }

        public string SourceName => "Guardian";

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
