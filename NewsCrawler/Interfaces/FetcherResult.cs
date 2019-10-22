using System;

namespace NewsCrawler.Interfaces
{
    public class FetcherResult
    {
        public string NewsSource { get; }

        public int ArticleLinksFound { get; }

        public int ArticlesSaved { get; }

        public int ErrorCounts { get; }

        public TimeSpan ElapsedTimeSpan { get; }

        public FetcherResult(string newsSource, int articleLinksFound, int articlesSaved, int errorCounts, TimeSpan elapsedTimeSpan)
        {
            NewsSource = newsSource ?? throw new ArgumentNullException(nameof(newsSource));
            ArticleLinksFound = articleLinksFound;
            ArticlesSaved = articlesSaved;
            ErrorCounts = errorCounts;
            ElapsedTimeSpan = elapsedTimeSpan;
        }

        public static FetcherResult operator+(FetcherResult a, FetcherResult b)
        {
            if (a == null) {
                return b;
            }
            if (b == null)
            {
                return b;
            }

            string aggregateNewsSource = string.Join(", ", a.NewsSource, b.NewsSource);
            int aggregateArticleLinksFound = a.ArticleLinksFound + b.ArticleLinksFound;
            int aggregateArticlesSaved = a.ArticlesSaved + b.ArticlesSaved;
            int aggregateErrorCount = a.ErrorCounts + b.ErrorCounts;
            var aggregateElapsedTimeSpan = a.ElapsedTimeSpan + b.ElapsedTimeSpan;

            var aggregate = new FetcherResult(aggregateNewsSource, aggregateArticleLinksFound, aggregateArticlesSaved, aggregateErrorCount, aggregateElapsedTimeSpan);
            return aggregate;
        }
    }
}
