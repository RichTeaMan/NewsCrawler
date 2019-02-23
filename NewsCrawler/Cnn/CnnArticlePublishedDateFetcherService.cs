using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using System;
using System.Linq;

namespace NewsCrawler.Cnn
{
    public class CnnArticlePublishedDateFetcherService : IArticlePublishedDateFetcherService
    {
        public DateTimeOffset? FetchDate(string articleContent)
        {
            DateTimeOffset? dateTimeOffset = null;

            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var dateNode = doc.DocumentNode.Descendants().FirstOrDefault(n => HasDateAttribute(n));
            string dateStr = dateNode?.GetAttributeValue("content", null);

            if (!string.IsNullOrEmpty(dateStr))
            {
                DateTimeOffset parsedDate;
                if (DateTimeOffset.TryParse(dateStr, out parsedDate))
                {
                    dateTimeOffset = parsedDate;
                }
            }
            return dateTimeOffset;
        }

        private bool HasDateAttribute(HtmlNode htmlNode)
        {
            return htmlNode.Name == "meta" && htmlNode.Attributes.Any(attr => attr.Name == "itemprop" && attr.Value == "datePublished");
        }
    }
}
