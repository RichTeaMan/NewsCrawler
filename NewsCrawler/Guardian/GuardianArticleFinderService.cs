using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;

namespace NewsCrawler.Guardian
{
    public class GuardianArticleFinderService : AbstractArticleFinderService
    {
        protected override string BaseUrl => "https://www.theguardian.com/";

        protected override string[] FetchIndexUrls()
        {
            return new[]
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
        }

        public override string SourceName => "Guardian";

        public GuardianArticleFinderService(ILogger<GuardianArticleFinderService> logger, INewsArticleDeterminationService newsArticleDeterminationService)
            :base(logger, newsArticleDeterminationService)
        { }
    }
}
