﻿using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using System;
using System.Linq;

namespace NewsCrawler.DailyMail
{
    public class DailyMailNewsArticlePublishedDateFetcherService : IArticlePublishedDateFetcherService
    {
        public DateTimeOffset? FetchDate(string articleContent)
        {
            DateTimeOffset? dateTimeOffset = null;

            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var dateNode = doc.DocumentNode.Descendants().FirstOrDefault(n => HasDateAttribute(n));
            string dateStrSeconds = dateNode?.GetAttributeValue("data-seconds", null);

            long seconds;
            if (!string.IsNullOrEmpty(dateStrSeconds) && long.TryParse(dateStrSeconds, out seconds))
            {
                dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds);
            }
            return dateTimeOffset;
        }

        private bool HasDateAttribute(HtmlNode htmlNode)
        {
            return htmlNode.Attributes.Any(attr => attr.Name == "class" && (attr.Value == "date date--v2" || attr.Value?.Contains("vxp-date") == true));
        }
    }
}