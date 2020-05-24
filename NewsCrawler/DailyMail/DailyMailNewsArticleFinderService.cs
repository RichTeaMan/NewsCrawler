using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;

namespace NewsCrawler.DailyMail
{
    public class DailyMailNewsArticleFinderService : AbstractArticleFinderService
    {
        protected override string BaseUrl => "https://www.dailymail.co.uk";

        protected override string[] FetchIndexUrls()
        {
            return new[]
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
        }

        public override string SourceName => "Daily Mail";

        public DailyMailNewsArticleFinderService(ILogger<DailyMailNewsArticleFinderService> logger, INewsArticleDeterminationService newsArticleDeterminationService)
            :base(logger, newsArticleDeterminationService)
        { }
    }
}
