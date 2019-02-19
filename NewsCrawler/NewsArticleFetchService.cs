using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
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

        public NewsArticleFetchService(
            INewsArticleTitleFetcherService newsArticleTitleFetcherService,
            IArticlePublishedDateFetcherService articlePublishedDateFetcherService,
            INewsArticleDeterminationService newsArticleDeterminationService)
        {
            this.newsArticleTitleFetcherService = newsArticleTitleFetcherService ?? throw new ArgumentNullException(nameof(newsArticleTitleFetcherService));
            this.articlePublishedDateFetcherService = articlePublishedDateFetcherService ?? throw new ArgumentNullException(nameof(articlePublishedDateFetcherService));
            this.newsArticleDeterminationService = newsArticleDeterminationService ?? throw new ArgumentNullException(nameof(newsArticleDeterminationService));
        }

        public async Task<Article> FetchArticleAsync(string url)
        {
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                string title = Truncate(newsArticleTitleFetcherService.FetchTitle(content), Constants.MAX_TITLE_LENGTH);
                var publishedDate = articlePublishedDateFetcherService.FetchDate(content);
                var article = new Article
                {
                    Content = content,
                    Title = title,
                    Url = Truncate(url, Constants.MAX_URL_LENGTH),
                    RecordedDate = DateTimeOffset.Now,
                    PublishedDate = publishedDate
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
