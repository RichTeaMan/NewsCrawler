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
            "https://www.bbc.co.uk/news/business",
            "https://www.bbc.co.uk/news/business/companies",
            "https://www.bbc.co.uk/news/business/economy",
            "https://www.bbc.co.uk/news/business/market-data",
            "https://www.bbc.co.uk/news/business/markets/asiapacific/tsenik_tokn",
            "https://www.bbc.co.uk/news/business/markets/europe/lse_ukx",
            "https://www.bbc.co.uk/news/business/markets/us/djse_indu",
            "https://www.bbc.co.uk/news/business/markets/us/nasdaq_ixic",
            "https://www.bbc.co.uk/news/business/your_money",
            "https://www.bbc.co.uk/news/disability",
            "https://www.bbc.co.uk/news/education",
            "https://www.bbc.co.uk/news/england",
            "https://www.bbc.co.uk/news/england/beds_bucks_and_herts",
            "https://www.bbc.co.uk/news/england/berkshire",
            "https://www.bbc.co.uk/news/england/birmingham_and_black_country",
            "https://www.bbc.co.uk/news/england/bristol",
            "https://www.bbc.co.uk/news/england/cambridgeshire",
            "https://www.bbc.co.uk/news/england/cornwall",
            "https://www.bbc.co.uk/news/england/coventry_and_warwickshire",
            "https://www.bbc.co.uk/news/england/cumbria",
            "https://www.bbc.co.uk/news/england/derbyshire",
            "https://www.bbc.co.uk/news/england/devon",
            "https://www.bbc.co.uk/news/england/dorset",
            "https://www.bbc.co.uk/news/england/essex",
            "https://www.bbc.co.uk/news/england/gloucestershire",
            "https://www.bbc.co.uk/news/england/hampshire",
            "https://www.bbc.co.uk/news/england/hereford_and_worcester",
            "https://www.bbc.co.uk/news/england/humberside",
            "https://www.bbc.co.uk/news/england/kent",
            "https://www.bbc.co.uk/news/england/lancashire",
            "https://www.bbc.co.uk/news/england/leeds_and_west_yorkshire",
            "https://www.bbc.co.uk/news/england/leicester",
            "https://www.bbc.co.uk/news/england/lincolnshire",
            "https://www.bbc.co.uk/news/england/london",
            "https://www.bbc.co.uk/news/england/manchester",
            "https://www.bbc.co.uk/news/england/merseyside",
            "https://www.bbc.co.uk/news/england/norfolk",
            "https://www.bbc.co.uk/news/england/northamptonshire",
            "https://www.bbc.co.uk/news/england/nottingham",
            "https://www.bbc.co.uk/news/england/oxford",
            "https://www.bbc.co.uk/news/england/regions",
            "https://www.bbc.co.uk/news/england/shropshire",
            "https://www.bbc.co.uk/news/england/somerset",
            "https://www.bbc.co.uk/news/england/south_yorkshire",
            "https://www.bbc.co.uk/news/england/stoke_and_staffordshire",
            "https://www.bbc.co.uk/news/england/suffolk",
            "https://www.bbc.co.uk/news/england/surrey",
            "https://www.bbc.co.uk/news/england/sussex",
            "https://www.bbc.co.uk/news/england/tees",
            "https://www.bbc.co.uk/news/england/tyne_and_wear",
            "https://www.bbc.co.uk/news/england/wiltshire",
            "https://www.bbc.co.uk/news/england/york_and_north_yorkshire",
            "https://www.bbc.co.uk/news/entertainment_and_arts",
            "https://www.bbc.co.uk/news/explainers",
            "https://www.bbc.co.uk/news/have_your_say",
            "https://www.bbc.co.uk/news/health",
            "https://www.bbc.co.uk/news/in_pictures",
            "https://www.bbc.co.uk/news/localnews",
            "https://www.bbc.co.uk/news/newsbeat",
            "https://www.bbc.co.uk/news/northern_ireland",
            "https://www.bbc.co.uk/news/northern_ireland/northern_ireland_politics",
            "https://www.bbc.co.uk/news/politics",
            "https://www.bbc.co.uk/news/politics/parliaments",
            "https://www.bbc.co.uk/news/politics/uk_leaves_the_eu",
            "https://www.bbc.co.uk/news/popular/read",
            "https://www.bbc.co.uk/news/science_and_environment",
            "https://www.bbc.co.uk/news/scotland",
            "https://www.bbc.co.uk/news/scotland/edinburgh_east_and_fife",
            "https://www.bbc.co.uk/news/scotland/glasgow_and_west",
            "https://www.bbc.co.uk/news/scotland/highlands_and_islands",
            "https://www.bbc.co.uk/news/scotland/north_east_orkney_and_shetland",
            "https://www.bbc.co.uk/news/scotland/scotland_business",
            "https://www.bbc.co.uk/news/scotland/scotland_politics",
            "https://www.bbc.co.uk/news/scotland/south_scotland",
            "https://www.bbc.co.uk/news/scotland/tayside_and_central",
            "https://www.bbc.co.uk/news/special_reports",
            "https://www.bbc.co.uk/news/stories",
            "https://www.bbc.co.uk/news/technology",
            "https://www.bbc.co.uk/news/the_reporters",
            "https://www.bbc.co.uk/news/uk",
            "https://www.bbc.co.uk/news/video_and_audio/headlines",
            "https://www.bbc.co.uk/news/wales",
            "https://www.bbc.co.uk/news/wales/mid_wales",
            "https://www.bbc.co.uk/news/wales/north_east_wales",
            "https://www.bbc.co.uk/news/wales/north_west_wales",
            "https://www.bbc.co.uk/news/wales/south_east_wales",
            "https://www.bbc.co.uk/news/wales/south_west_wales",
            "https://www.bbc.co.uk/news/wales/wales_politics",
            "https://www.bbc.co.uk/news/world",
            "https://www.bbc.co.uk/news/world/africa",
            "https://www.bbc.co.uk/news/world/asia",
            "https://www.bbc.co.uk/news/world/asia/china",
            "https://www.bbc.co.uk/news/world/asia/india",
            "https://www.bbc.co.uk/news/world/australia",
            "https://www.bbc.co.uk/news/world/europe",
            "https://www.bbc.co.uk/news/world/europe/guernsey",
            "https://www.bbc.co.uk/news/world/europe/isle_of_man",
            "https://www.bbc.co.uk/news/world/europe/jersey",
            "https://www.bbc.co.uk/news/world/latin_america",
            "https://www.bbc.co.uk/news/world/middle_east",
            "https://www.bbc.co.uk/news/world/us_and_canada",
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
