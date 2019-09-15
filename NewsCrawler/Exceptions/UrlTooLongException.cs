using NewsCrawler.Persistence.Postgres;
using System;

namespace NewsCrawler.Exceptions
{
    public class UrlTooLongException : Exception
    {
        public string Url { get; }

        public UrlTooLongException(string url) : base($"URL is too long. {url.Length} > {Constants.MAX_URL_LENGTH}. URL: \"{url}\"") {
            Url = url;
        }
    }
}
