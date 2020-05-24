using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class ArticleStoreRunner : IDisposable, IArticleStoreRunner
    {
        private readonly ILogger logger;

        private readonly IServiceProvider serviceProvider;

        private readonly HttpClient httpClient = new HttpClient();

        private readonly string documentUrl = "https://document-store.richteaman.com/document";

        private readonly int maxAttempts = 10;

        public ArticleStoreRunner(ILogger<ArticleStoreRunner> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task StoreArticles()
        {
            logger.LogInformation("Storing all articles.");
            var articleBatcher = new ArticleBatcher(serviceProvider)
            {
                IncludeContent = true
            };

            await articleBatcher.RunArticleBatch(
            article => article.ArticleContent != null && article.ArticleContent.Content != string.Empty,
            async article =>
            {

                // curl - X PUT http://localhost:5002/document -F "document=@amazon.sh" -H "key: test-docker" -v

                string articleText = article.ArticleContent.Content;
                string articleKey = article.Url;

                bool deleteArticle = false;
                using (var content = new StringContent(articleText, Encoding.UTF8, "application/x-www-form-urlencoded"))
                {
                    content.Headers.Add("key", articleKey);

                    int attempts = 0;
                    while (attempts < maxAttempts)
                    {
                        // probably needs a DDOS guard
                        using (var response = await httpClient.PutAsync(documentUrl, content))
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                            {
                                logger.LogWarning($"Article with  key '{articleKey}' has already been logged.");
                                break;
                            }
                            else if (response.IsSuccessStatusCode)
                            {
                                //logger.LogDebug($"Article with  key '{articleKey}' successfully stored.");
                                deleteArticle = true;
                                break;
                            }
                            else
                            {
                                logger.LogError($"Unknown response code '{response.StatusCode}' - {response.ReasonPhrase}");
                                logger.LogError(response.ToString());
                            }
                        }
                        attempts++;
                    }
                }

                if (deleteArticle)
                {
                    // come up with a real way to delete rows.
                    article.ArticleContent.Content = string.Empty;
                }
                return deleteArticle;

            });

            logger.LogInformation("Completed storing articles.");
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }

}
