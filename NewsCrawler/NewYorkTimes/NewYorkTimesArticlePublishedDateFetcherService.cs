using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using System;
using System.Linq;

namespace NewsCrawler.NewYorkTimes
{
    public class NewYorkTimesArticlePublishedDateFetcherService : IArticlePublishedDateFetcherService
    {
        public DateTimeOffset? FetchDate(string articleContent)
        {
            DateTimeOffset? dateTimeOffset = null;

            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var dateNode = doc.DocumentNode.Descendants().FirstOrDefault(n => HasDateAttribute(n));
            string dateTimeStr = dateNode?.GetAttributeValue("datetime", null);

            DateTime dateTime;
            if (!string.IsNullOrEmpty(dateTimeStr) && DateTime.TryParse(dateTimeStr, out dateTime))
            {
                dateTimeOffset = dateTime;
            }
            return dateTimeOffset;
        }

        private bool HasDateAttribute(HtmlNode htmlNode)
        {
            return htmlNode.Name == "time" && htmlNode.Attributes.Any(attr => attr.Name == "datetime" && !string.IsNullOrEmpty(attr.Value));
        }
    }
}
