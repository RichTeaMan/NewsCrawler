using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class WordCountService : IWordCountService
    {
        private readonly string[] symbolsToClean = new[] { "”", "”", "“", "’s", "'s", ",", ".", "!", "?", "%", "£", "$", "(", ")", "/", "\"", ":", ";" };

        private readonly IServiceProvider serviceProvider;

        public WordCountService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task UpdateWordCount()
        {
            Console.WriteLine("Updating word count...");
            var wordsByNewsSource = new Dictionary<string, Dictionary<string, int>>();

            int articleCount = 0;
            using (var context = serviceProvider.GetRequiredService<NewsArticleContext>())
            {
                articleCount = context.Articles.Count();
            }
            Console.WriteLine($"Updating word count for {articleCount} articles");
            int step = articleCount / 100;
            int count = 0;

            var articleBatch = new ArticleBatcher(serviceProvider);
            await articleBatch.RunArticleBatch(
                article => true,
                article =>
                {

                    Dictionary<string, int> words;
                    if (!wordsByNewsSource.TryGetValue(article.NewsSource, out words))
                    {
                        words = new Dictionary<string, int>();
                        wordsByNewsSource.Add(article.NewsSource, words);
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
                        Console.WriteLine($"Article {article.Id} does not have a cleaned article.");
                    }
                    count++;
                    if (count % step == 0)
                    {
                        Console.WriteLine($"Completed {count} of {articleCount} articles.");
                    }

                    return Task.FromResult(false);
                });
            Console.WriteLine($"Completed {count} of {articleCount} articles.");

            Console.WriteLine("Updating word counts...");
            using (var context = serviceProvider.GetRequiredService<NewsArticleContext>())
            {
                context.WordCount.RemoveRange(context.WordCount.ToArray());
                await context.SaveChangesAsync();
                Console.WriteLine("Removed old word counts.");

                foreach (var words in wordsByNewsSource)
                {
                    string newsSource = words.Key;
                    foreach (var word in words.Value)
                    {

                        var wordCount = new WordCount { NewsSource = newsSource, Word = word.Key, Count = word.Value };
                        if (wordCount.Word.Length > Constants.MAX_WORD_LENGTH)
                        {
                            Console.WriteLine($"Word '{wordCount.Word}' is longer than the maximum word length.");
                        }
                        else
                        {
                            context.WordCount.Add(wordCount);
                        }
                    }
                }
                Console.WriteLine("Committing word counts...");
                await context.SaveChangesAsync();
                Console.WriteLine("Updating word count complete.");
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
