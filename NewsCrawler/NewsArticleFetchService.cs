using Microsoft.Extensions.Logging;
using NewsCrawler.Exceptions;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class NewsArticleFetchService : INewsArticleFetchService
    {
        private readonly ILogger logger;
        private readonly HttpClient httpClient = new HttpClient();
        private readonly INewsArticleTitleFetcherService newsArticleTitleFetcherService;
        private readonly IArticlePublishedDateFetcherService articlePublishedDateFetcherService;
        private readonly INewsArticleDeterminationService newsArticleDeterminationService;
        private readonly IArticleCleaner articleCleaner;

        private const string DOCUMENT_STORE_URL = "https://document-store.richteaman.com/document?key=";

        public NewsArticleFetchService(
            ILogger<NewsArticleFetchService> logger,
            INewsArticleTitleFetcherService newsArticleTitleFetcherService,
            IArticlePublishedDateFetcherService articlePublishedDateFetcherService,
            INewsArticleDeterminationService newsArticleDeterminationService,
            IArticleCleaner articleCleaner)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.newsArticleTitleFetcherService = newsArticleTitleFetcherService ?? throw new ArgumentNullException(nameof(newsArticleTitleFetcherService));
            this.articlePublishedDateFetcherService = articlePublishedDateFetcherService ?? throw new ArgumentNullException(nameof(articlePublishedDateFetcherService));
            this.newsArticleDeterminationService = newsArticleDeterminationService ?? throw new ArgumentNullException(nameof(newsArticleDeterminationService));
            this.articleCleaner = articleCleaner ?? throw new ArgumentNullException(nameof(articleCleaner));
        }

        private async Task<string> FetchContentFromUrl(string url)
        {
            logger.LogDebug($"Attempting to download from '{url}'.");
            using var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> FetchContent(string url)
        {
            Exception lastException = null;
            string content = null;
            // try doc store
            string docUrl = DOCUMENT_STORE_URL + url;

            // list of urls to try, in order.
            List<string> urls = new List<string>();
            urls.Add(docUrl);
            urls.Add(url);

            foreach (var attemptUrl in urls)
            {
                try
                {
                    content = await FetchContentFromUrl(attemptUrl);
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Failed to get '{attemptUrl}': {ex.Message}.");
                    lastException = ex;
                }
            }
            if (content == null)
            {
                throw lastException;
            }

            return content;
        }

        public async Task<Article> FetchArticleAsync(string url)
        {
            if (url.Length >= Constants.MAX_URL_LENGTH)
            {
                throw new UrlTooLongException(url);
            }
            try
            {
                var content = await FetchContent(url);
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
                    Title = title,
                    Url = Truncate(url, Constants.MAX_URL_LENGTH),
                    RecordedDate = DateTimeOffset.Now,
                    PublishedDate = publishedDate,
                    CleanedContentLength = cleanedArticleLength,
                    ContentLength = contentLength,
                    ArticleCleanedContent = new ArticleCleanedContent() { CleanedContent = cleanedArticle }
                };
                article.IsIndexPage = newsArticleDeterminationService.IsIndexPage(article.Url);

                return article;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpClientException(url, ex);
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
