﻿using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewsCrawler
{
    public class SpacyWordCountService : IWordCountService, IDisposable
    {
        private const string DOCUMENT_SCANNER_URL = "http://localhost:5002/scanUpload";

        private readonly IServiceProvider serviceProvider;

        private readonly HttpClient client = new HttpClient();

        public SpacyWordCountService(IServiceProvider serviceProvider)
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

            var articleBatch = new ArticleBatcher(serviceProvider) { ConcurrentArticlePredicates = 5 };
            await articleBatch.RunArticleBatch(
                article => !article.IsIndexPage,
                async article =>
                {
                    if (!string.IsNullOrWhiteSpace(article.CleanedContent))
                    {
                        var payload = Encoding.UTF8.GetBytes(article.CleanedContent);                       
                        var multipartContent = new MultipartFormDataContent();
                        multipartContent.Add(new ByteArrayContent(payload), "document", "document.txt");

                        using (var response = await client.PostAsync(DOCUMENT_SCANNER_URL, multipartContent))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                                using (StreamReader reader = new StreamReader(responseStream))
                                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                                {
                                    JsonSerializer ser = new JsonSerializer();
                                    var scanResponse = ser.Deserialize<ScanReponse>(jsonReader);
                                    Dictionary<string, int> words;
                                    if (!wordsByNewsSource.TryGetValue(article.NewsSource, out words))
                                    {
                                        words = new Dictionary<string, int>();
                                        wordsByNewsSource.Add(article.NewsSource, words);
                                    }
                                    foreach (var word in scanResponse.tokens.Select(t => t.lemma))
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
                            }
                            else
                            {
                                Console.WriteLine($"Could not conact document service: {response.StatusCode}");
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

                    return false;
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client?.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }


    public class ScanReponse
    {
        public Token[] tokens { get; set; }
    }

    public class Token
    {
        public string dep { get; set; }
        public bool isAlpha { get; set; }
        public bool isStop { get; set; }
        public string lemma { get; set; }
        public string pos { get; set; }
        public string shape { get; set; }
        public string tag { get; set; }
        public string text { get; set; }
    }

}