using NewsCrawler.Exceptions;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class NewsArticleFetchService : INewsArticleFetchService
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly INewsArticleTitleFetcherService newsArticleTitleFetcherService;
        private readonly IArticlePublishedDateFetcherService articlePublishedDateFetcherService;
        private readonly INewsArticleDeterminationService newsArticleDeterminationService;
        private readonly IArticleCleaner articleCleaner;

        public NewsArticleFetchService(
            INewsArticleTitleFetcherService newsArticleTitleFetcherService,
            IArticlePublishedDateFetcherService articlePublishedDateFetcherService,
            INewsArticleDeterminationService newsArticleDeterminationService,
            IArticleCleaner articleCleaner)
        {
            this.newsArticleTitleFetcherService = newsArticleTitleFetcherService ?? throw new ArgumentNullException(nameof(newsArticleTitleFetcherService));
            this.articlePublishedDateFetcherService = articlePublishedDateFetcherService ?? throw new ArgumentNullException(nameof(articlePublishedDateFetcherService));
            this.newsArticleDeterminationService = newsArticleDeterminationService ?? throw new ArgumentNullException(nameof(newsArticleDeterminationService));
            this.articleCleaner = articleCleaner ?? throw new ArgumentNullException(nameof(articleCleaner));
        }

        public async Task<Article> FetchArticleAsync(string url)
        {
            if (url.Length >= Constants.MAX_URL_LENGTH)
            {
                throw new UrlTooLongException(url);
            }
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                string title = Truncate(newsArticleTitleFetcherService.FetchTitle(content), Constants.MAX_TITLE_LENGTH);
                var publishedDate = articlePublishedDateFetcherService.FetchDate(content);
                var cleanedArticle = articleCleaner.CleanArticle(content);
                int cleanedArticleLength = 0;
                int contentLength = 0;

                if (!string.IsNullOrEmpty(cleanedArticle))
                {
                    cleanedArticleLength = cleanedArticle.Length;
                }
                if (!string.IsNullOrEmpty(content))
                {
                    contentLength = content.Length;
                }

                var article = new Article
                {
                    Content = content,
                    Title = title,
                    Url = Truncate(url, Constants.MAX_URL_LENGTH),
                    RecordedDate = DateTimeOffset.Now,
                    PublishedDate = publishedDate,
                    CleanedContent = cleanedArticle,
                    CleanedContentLength = cleanedArticleLength,
                    ContentLength = contentLength
                };
                article.IsIndexPage = newsArticleDeterminationService.IsIndexPage(article.Url);

                return article;
            }
            else
            {
                throw new Exception($"{response.StatusCode} Could not fetch from {url}.");
            }
        }

        private string Truncate(string value, int maxLength)
        {
            if (value == null)
            {
                return null;
            }
            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}
