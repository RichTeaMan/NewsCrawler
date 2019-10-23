﻿using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;

namespace NewsCrawler.Cnn
{
    public class CnnArticleFinderService : AbstractArticleFinderService
    {
        protected override string BaseUrl => "https://edition.cnn.com";

        protected override string[] FetchIndexUrls()
        {
            return new[]
            {
                "https://edition.cnn.com/",
                "https://edition.cnn.com/specials/opinions/two-degrees",
                "https://edition.cnn.com/specials/africa/inside-africa",
                "https://edition.cnn.com/specials/impact-your-world",
                "https://edition.cnn.com/specials/world/freedom-project",
                "https://edition.cnn.com/specials",
                "https://edition.cnn.com/specials/health/live-longer",
                "https://edition.cnn.com/specials/health/parenting",
                "https://edition.cnn.com/specials/health/wellness",
                "https://edition.cnn.com/specials/health/fitness-excercise",
                "https://edition.cnn.com/specials/health/food-diet",
                "https://edition.cnn.com/style/videos",
                "https://edition.cnn.com/style/autos",
                "https://edition.cnn.com/style/luxury",
                "https://edition.cnn.com/style/architecture",
                "https://edition.cnn.com/style/fashion",
                "https://edition.cnn.com/style/design",
                "https://edition.cnn.com/style/arts",
                "https://edition.cnn.com/travel/videos",
                "https://edition.cnn.com/travel/stay",
                "https://edition.cnn.com/travel/play",
                "https://edition.cnn.com/travel/food-and-drink",
                "https://edition.cnn.com/specials/cnn-heroes",
                "https://edition.cnn.com/travel/destinations",
                "https://edition.cnn.com/specials/latest-news-videos",
                "https://edition.cnn.com/specials/videos/hln",
                "https://edition.cnn.com/msa",
                "https://edition.cnn.com/accessibility",
                "https://edition.cnn.com/privacy",
                "https://edition.cnn.com/terms",
                "https://edition.cnn.com/specials/world/specials-page-cnn-partners",
                "https://edition.cnn.com/specials/more/cnn-leadership",
                "https://edition.cnn.com/specials/profiles",
                "https://edition.cnn.com/weather",
                "https://edition.cnn.com/more",
                "https://edition.cnn.com/specials/vr/vr-archives",
                "https://edition.cnn.com/2017/03/04/vr/how-to-watch-vr",
                "https://edition.cnn.com/vr",
                "https://edition.cnn.com/specials/tv/anchors-and-reporters",
                "https://edition.cnn.com/tv/schedule/europe",
                "https://edition.cnn.com/tv/shows",
                "https://edition.cnn.com/specials/international-video-landing/feature-show-videos",
                "https://edition.cnn.com/specials/sport/world-rugby",
                "https://edition.cnn.com/sport/sailing",
                "https://edition.cnn.com/sport/motorsport",
                "https://edition.cnn.com/specials/politics/president-donald-trump-45",
                "https://edition.cnn.com/uk",
                "https://edition.cnn.com/india",
                "https://edition.cnn.com/middle-east",
                "https://edition.cnn.com/europe",
                "https://edition.cnn.com/china",
                "https://edition.cnn.com/australia",
                "https://edition.cnn.com/asia",
                "https://edition.cnn.com/americas",
                "https://edition.cnn.com/africa",
                "https://edition.cnn.com/us",
                "https://edition.cnn.com/videos",
                "https://edition.cnn.com/health",
                "https://edition.cnn.com/style",
                "https://edition.cnn.com/travel",
                "https://edition.cnn.com/sport",
                "https://edition.cnn.com/entertainment",
                "https://edition.cnn.com/business",
                "https://edition.cnn.com/politics",
                "https://edition.cnn.com/world",
                "https://edition.cnn.com/specials/politics/congress-capitol-hill",
                "https://edition.cnn.com/specials/politics/supreme-court-nine",
                "https://edition.cnn.com/election/2018/results",
                "https://edition.cnn.com/sport/horse-racing",
                "https://edition.cnn.com/sport/skiing",
                "https://edition.cnn.com/sport/golf",
                "https://edition.cnn.com/sport/equestrian",
                "https://edition.cnn.com/sport/tennis",
                "https://edition.cnn.com/sport/football",
                "https://edition.cnn.com/specials/tech/innovative-cities",
                "https://edition.cnn.com/specials/tech/work-transformed",
                "https://edition.cnn.com/specials/tech/business-evolved",
                "https://edition.cnn.com/specials/tech/upstarts",
                "https://edition.cnn.com/about",
                "https://edition.cnn.com/specials/tech/mission-ahead",
                "https://edition.cnn.com/specials/tech/innovate",
                "https://edition.cnn.com/entertainment/culture",
                "https://edition.cnn.com/entertainment/tv-shows",
                "https://edition.cnn.com/entertainment/movies",
                "https://edition.cnn.com/entertainment/celebrities",
                "https://edition.cnn.com/business/videos",
                "https://edition.cnn.com/business/perspectives",
                "https://edition.cnn.com/business/success",
                "https://edition.cnn.com/business/media",
                "https://edition.cnn.com/business/tech",
                "https://edition.cnn.com/specials/tech/gadget",
                "https://edition.cnn.com/newsletters",
            };
        }

        public override string SourceName => "CNN";

        public CnnArticleFinderService(ILogger<CnnArticleFinderService> logger, INewsArticleDeterminationService newsArticleDeterminationService)
            : base(logger, newsArticleDeterminationService)
        { }

    }
}
