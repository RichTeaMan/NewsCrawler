using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using System;
using System.Linq;

namespace NewsCrawler.Guardian
{
    public class GuardianArticlePublishedDateFetcherService : IArticlePublishedDateFetcherService
    {
        public DateTimeOffset? FetchDate(string articleContent)
        {
            DateTimeOffset? dateTimeOffset = null;

            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var dateNode = doc.DocumentNode.Descendants().FirstOrDefault(n => HasDateAttribute(n));
            string dateStrMilliSeconds = dateNode?.GetAttributeValue("data-timestamp", null);

            long milliSeconds;
            if (!string.IsNullOrEmpty(dateStrMilliSeconds) && long.TryParse(dateStrMilliSeconds, out milliSeconds))
            {
                long seconds = milliSeconds / 1000;
                dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds);
            }
            return dateTimeOffset;
        }

        private bool HasDateAttribute(HtmlNode htmlNode)
        {

            return htmlNode.Name == "time" && htmlNode.Attributes.FirstOrDefault(attr => attr.Name == "itemprop")?.Value == "datePublished";
        }
    }
}
