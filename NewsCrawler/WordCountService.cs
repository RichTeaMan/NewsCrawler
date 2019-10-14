using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class WordCountService : IWordCountService
    {
        private readonly string[] symbolsToClean = new[] { "”", "”", "“", "’s", "'s", ",", ".", "!", "?", "%", "£", "$", "(", ")", "/", "\"", ":", ";" };

        private readonly ILogger logger;

        private readonly IServiceProvider serviceProvider;

        public WordCountService(ILogger<WordCountService> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task UpdateWordCount()
        {
            logger.LogInformation("Updating word count...");
            var wordsByNewsSource = new Dictionary<Source, Dictionary<string, int>>();

            int articleCount = 0;
            using (var context = serviceProvider.GetRequiredService<PostgresNewsArticleContext>())
            {
                articleCount = context.Articles.Count();
            }
            logger.LogInformation($"Updating word count for {articleCount} articles");
            int step = articleCount / 100;
            int count = 0;

            var articleBatch = new ArticleBatcher(serviceProvider);
            await articleBatch.RunArticleBatch(
                article => true,
                article =>
                {

                    Dictionary<string, int> words;
                    if (!wordsByNewsSource.TryGetValue(article.Source, out words))
                    {
                        words = new Dictionary<string, int>();
                        wordsByNewsSource.Add(article.Source, words);
                    }

                    var splitWords = article.CleanedContent?.Split(' ');
                    if (splitWords != null)
                    {
                        foreach (var word in splitWords.Select(w => cleanString(w)).Where(w => !string.IsNullOrEmpty(w)))
                        {
                            if (words.ContainsKey(word))
                            {
                                words[word]++;
                            }
                            else
                            {
                                words.Add(word, 1);
                            }
                        }
                    }
                    else
                    {
                        logger.LogInformation($"Article {article.Id} does not have a cleaned article.");
                    }
                    count++;
                    if (count % step == 0)
                    {
                        logger.LogInformation($"Completed {count} of {articleCount} articles.");
                    }

                    return Task.FromResult(false);
                });
            logger.LogInformation($"Completed {count} of {articleCount} articles.");

            logger.LogInformation("Updating word counts...");
            using (var context = serviceProvider.GetRequiredService<PostgresNewsArticleContext>())
            {
                context.WordCount.RemoveRange(context.WordCount.ToArray());
                await context.SaveChangesAsync();
                logger.LogInformation("Removed old word counts.");

                foreach (var words in wordsByNewsSource)
                {
                    string newsSource = words.Key.Name;
                    foreach (var word in words.Value)
                    {

                        var wordCount = new WordCount { NewsSource = newsSource, Word = word.Key, Count = word.Value };
                        if (wordCount.Word.Length > Constants.MAX_WORD_LENGTH)
                        {
                            logger.LogWarning($"Word '{wordCount.Word}' is longer than the maximum word length.");
                        }
                        else
                        {
                            context.WordCount.Add(wordCount);
                        }
                    }
                }
                logger.LogInformation("Committing word counts...");
                await context.SaveChangesAsync();
                logger.LogInformation("Updating word count complete.");
            }
        }

        private string cleanString(string value)
        {
            string result = value?.ToLowerInvariant()?.Trim();
            if (!string.IsNullOrEmpty(result))
            {
                foreach (string symbol in symbolsToClean)
                {
                    result = result.Replace(symbol, string.Empty);
                }
            }
            return result;
        }
    }
}
